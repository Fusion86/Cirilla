using Cirilla.Moji;
using Cirilla.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cirilla.WPF.Controls
{
    /// <summary>
    /// Interaction logic for MojiRenderer.xaml
    /// </summary>
    public partial class MojiRenderer : UserControl
    {
        public MojiRenderer()
        {
            InitializeComponent();
        }

        private readonly MojiParser parser = new MojiParser();
        private readonly Dictionary<MojiColor, SolidColorBrush> brushCache = new Dictionary<MojiColor, SolidColorBrush>();

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(MojiRenderer),
            new FrameworkPropertyMetadata("", OnTextChangedCallBack));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MojiRenderer ctrl)
                ctrl.OnTextChanged();
        }

        private void OnTextChanged()
        {
            Render();
        }

        private void Render()
        {
            var result = parser.Parse(Text);
            textBlock.Inlines.Clear();

            foreach (var part in result.Parts)
            {
                var el = CreateElement(part);
                textBlock.Inlines.Add(el);
            }

            foreach (var notice in result.Notices)
            {
                var el = CreateNoticeElement(notice);
                textBlock.Inlines.Add(el);
            }
        }

        private Run CreateElement(IMojiPart part)
        {
            if (part is MojiTextPart textPart)
            {
                var el = new Run();
                el.Text = textPart.Text;

                if (textPart.Color.HasValue)
                    el.Foreground = GetBrushForColor(textPart.Color.Value);

                if (textPart.FontSize.HasValue)
                    el.FontSize = textPart.FontSize.Value;

                return el;
            }
            else if (part is MojiIconPart iconPart)
            {
                // TODO: Implement icons.
                return new Run("�");
            }

            throw new NotImplementedException();
        }

        private Run CreateNoticeElement(MojiParserNotice notice)
        {
            return new Run
            {
                Text = $"{notice.Level.ToString().ToUpperInvariant()}: {notice.Message}\n",
                Foreground = GetBrushForColor(MojiColor.MOJI_YELLOW_DEFAULT)
            };
        }

        private SolidColorBrush GetBrushForColor(MojiColor color)
        {
            if (brushCache.TryGetValue(color, out var brush))
                return brush;

            brush = new SolidColorBrush(MojiUtility.GetColor(color).ToMediaColor());
            brushCache.Add(color, brush);
            return brush;
        }
    }
}
