using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Moji.Test
{
    [TestClass]
    public class MojiParserTests
    {
        private readonly MojiParser parser = new();

        [TestMethod]
        public void NonexistentTag()
        {
            string moji = @"<SIZE 17>I use { } instead of <angle> brackets

{STYL MOJI_[...]_DEFAULT}(your text here){/STYL}  
Replace default by DEFAULT2 or SELECTED2 for the 2 last

<STYL MOJI_RED_DEFAULT>RED </STYL>  
<STYL MOJI_GREEN_DEFAULT>GREEN </STYL>  
<STYL MOJI_BLUE_DEFAULT>BLUE </STYL>  
<STYL MOJI_PURPLE_DEFAULT>PURPLE</STYL>  
<STYL MOJI_YELLOW_DEFAULT>YELLOW </STYL>  
<STYL MOJI_ORANGE_DEFAULT>ORANGE </STYL>  
<STYL MOJI_LIGHTBLUE_DEFAULT>LIGHTBLUE </STYL>  
<STYL MOJI_LIGHTGREEN_DEFAULT>LIGHTGREEN </STYL>   
<STYL MOJI_LIGHTYELLOW_DEFAULT>LIGHTYELLOW  </STYL>  
<STYL MOJI_SLGREEN_DEFAULT>SLGREEN</STYL>  
<STYL MOJI_WHITE_DEFAULT>WHITE </STYL>  
<STYL MOJI_BLACK_DEFAULT>BLACK </STYL> 
<STYL MOJI_WHITE_DEFAULT2>WHITE_DEFAULT2</STYL>   
<STYL MOJI_WHITE_SELECTED2>WHITE_SELECTED2</STYL>   </SIZE>
<SIZE 17><STYL MOJI_PURPLE_DEFAULT>                                                                  By Popyhead/Lyn</STYL></SIZE>";
            var res = parser.Parse(moji);

            Assert.IsTrue(res.Notices.Count == 0);
        }
    }
}
