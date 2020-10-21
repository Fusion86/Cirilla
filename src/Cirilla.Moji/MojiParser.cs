using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Cirilla.Moji
{
    public class MojiParser
    {
        private enum ParserState
        {
            ReadString = 0,
            ReadTagIdentifier,
            ReadTagParameter,
        }

        public MojiParserResult Parse(string text)
        {
            var tagStack = new TagStack();
            var result = new MojiParserResult();

            // Crappy state machine variables
            ParserState state = ParserState.ReadString;
            string tagIdentifier = "";
            string tagParameter = "";
            string textPart = "";
            int pos = 0;

            // Only used for warning/error messages.
            int row = 1;
            int col = 0;

            for (; pos < text.Length; pos++)
            {
                char c = text[pos];

                // Increment column (character) counter, only used for warning/error messages.
                // This is reset to 0 every time a `\n` character is found.
                col++;

                // A '<' tells us that a tag type identifier is coming.
                // If the identifier is not a valid identifier the game will ignore it. It will render the closing character '>' normally.
                // TODO: Can you escape a '<' in some way? E.g. like this '\<'?
                if (c == '<')
                {
                    if (textPart.Length > 0)
                    {
                        // Create MojiPart for whatever that is read up untill now.
                        // It's important that a new TagStack is is created.
                        var part = new MojiTextPart(textPart, new TagStack(tagStack));
                        result.Parts.Add(part);
                        textPart = "";
                    }

                    state = ParserState.ReadTagIdentifier;
                    continue;
                }

                if (c == '>')
                {
                    if (state == ParserState.ReadTagIdentifier)
                    {
                        // If closing tag
                        if (tagIdentifier.StartsWith("/"))
                        {
                            string substr = tagIdentifier.Substring(1);
                            tagStack.Pop(substr);
                            tagIdentifier = "";
                            state = ParserState.ReadString;
                            continue;
                        }
                        else
                        {
                            result.Notices.Add(MojiParserNotice.CreateError($"Tag is closed before a parameter is given. Line: {row}   Char: {col}"));
                            break;
                        }
                    }

                    if (state == ParserState.ReadTagParameter)
                    {
                        // Tag identifier and parameter closed.

                        try
                        {
                            var tag = CreateTag(tagIdentifier, tagParameter, row, col);

                            if (tag is MojiIconTag iconTag)
                            {
                                // Create a MojiIconPart and don't push the tag to the tagStack.
                                var iconPart = new MojiIconPart(iconTag);
                                result.Parts.Add(iconPart);
                            }
                            else
                            {
                                // Push STYL and SIZE tags to the tagStack.
                                tagStack.Push(tag);
                            }
                        }
                        catch (Exception ex)
                        {
                            result.Notices.Add(MojiParserNotice.CreateError(ex.Message));
                            break;
                        }

                        state = ParserState.ReadString;
                        tagIdentifier = "";
                        tagParameter = "";
                        //textPart = "";
                        continue;
                    }
                }

                if (c == ' ')
                {
                    if (state == ParserState.ReadTagIdentifier)
                    {
                        state = ParserState.ReadTagParameter;
                        continue;
                    }

                    if (state == ParserState.ReadTagParameter)
                    {
                        result.Notices.Add(MojiParserNotice.CreateError($"Unexpected whitespace inside tag parameter. Line: {row}   Char: {col}"));
                        break;
                    }
                }

                if (c == '\n')
                {
                    // Only used for warning/error messages.
                    col = 0;
                    row++;
                }

                switch (state)
                {
                    case ParserState.ReadString:
                        textPart += c;
                        break;
                    case ParserState.ReadTagIdentifier:
                        tagIdentifier += c;
                        break;
                    case ParserState.ReadTagParameter:
                        tagParameter += c;
                        break;
                    default:
                        break;
                }
            }

            // Add text if textPart is not empty.
            // Usually when a new tag is read all the text that is read before it will be added to the Parts lists.
            // However when there are no tags in the string then this will never happen, 
            // to still add the read text in those cased we do this check here. 
            if (textPart != "")
            {
                var part = new MojiTextPart(textPart, new TagStack(tagStack));
                result.Parts.Add(part);
            }

            if (pos != text.Length || result.HasErrors)
            {
                // Show error when pos != text.Length (aka for-loop has been cancelled early)
                // Also show error when result.HasErrors (because in those cases the for-loop will also be cancelled), though this *should* be covered in the first check.
                result.Notices.Add(MojiParserNotice.CreateError("Couldn't parse the full text because an error was encountered."));
            }
            else if (!tagStack.IsEmpty)
            {
                // Show warning when tagStack != empty.
                // This happens when a tag is opened but not closed.
                // E.g. `<SIZE 17>Hello world.`, notice the lack of the `</SIZE>` tag.
                result.Notices.Add(MojiParserNotice.CreateWarning("Not all tags are closed."));
            }

            foreach (var unclosedTag in tagStack.Tags)
            {
                result.Notices.Add(MojiParserNotice.CreateWarning($"There is a <{unclosedTag.Type}> tag that does not have a matching </{unclosedTag.Type}> tag."));
            }

            return result;
        }

        private IMojiTag CreateTag(string tagIdentifier, string parameter, int row, int col)
        {
            if (Enum.TryParse<MojiType>(tagIdentifier, out var tag))
                return tag switch
                {
                    MojiType.SIZE => MojiSizeTag.Parse(parameter, row, col),
                    MojiType.STYL => MojiStyleTag.Parse(parameter, row, col),
                    MojiType.ICON => MojiIconTag.Parse(parameter, row, col),
                    _ => throw new NotImplementedException($"Unimplemented MojiType '{tag}'. Line: {row}   Char: {col}"),
                };
            else
                throw new MojiParserException($"Unrecognized tag identifier '{tagIdentifier}'. Line: {row}   Char: {col}");
        }
    }

    public class MojiParserException : Exception
    {
        public MojiParserException()
        {
        }

        public MojiParserException(string message) : base(message)
        {
        }

        public MojiParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MojiParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class MojiParserResult
    {
        public MojiParserResult(List<IMojiPart>? parts = null)
        {
            Parts = parts ?? new List<IMojiPart>();
            Notices = new List<MojiParserNotice>();
        }

        public List<IMojiPart> Parts { get; }
        public List<MojiParserNotice> Notices { get; }

        public bool HasErrors => Notices.FirstOrDefault(x => x.Level == MojiParserNoticeLevel.Error) != null;
        public bool HasWarnings => Notices.FirstOrDefault(x => x.Level == MojiParserNoticeLevel.Warning) != null;
    }

    public interface IMojiPart
    {

    }

    [DebuggerDisplay("{Text}")]
    public class MojiTextPart : IMojiPart
    {
        public MojiTextPart(string text, TagStack? tags = null)
        {
            Text = text;
            Tags = tags ?? new TagStack();
        }

        public string Text { get; }
        public TagStack Tags { get; }

        public int? FontSize => Tags.Get<MojiSizeTag>()?.Size;
        public MojiColor? Color => Tags.Get<MojiStyleTag>()?.Color;
    }

    public class MojiIconPart : IMojiPart
    {
        public MojiIconPart(MojiIconTag tag)
        {
            Tag = tag;
        }

        public MojiIconTag Tag { get; }
    }

    public class MojiParserNotice
    {
        public MojiParserNotice(MojiParserNoticeLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        public MojiParserNoticeLevel Level { get; }
        public string Message { get; }

        public static MojiParserNotice CreateWarning(string message)
        {
            return new MojiParserNotice(MojiParserNoticeLevel.Warning, message);
        }

        public static MojiParserNotice CreateError(string message)
        {
            return new MojiParserNotice(MojiParserNoticeLevel.Warning, message);
        }
    }

    public enum MojiParserNoticeLevel
    {
        Warning,
        Error,
    }
}
