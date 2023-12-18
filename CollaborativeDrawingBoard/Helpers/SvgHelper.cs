using System.Text.RegularExpressions;

namespace CollaborativeDrawingBoard.Helpers
{
    public static class SvgHelper
    {
        public static string GetEmptySvgWithUserScreen(int screenWidth, int screenHeight)
        {
            return $"<svg xmlns='http://www.w3.org/2000/svg' width='{screenWidth}' height='{screenHeight}'></svg>";
        }

        public static string ChangeViewSize(string svg, int newWidth, int newHeight)
        {
            var regex = new Regex(@"width=['""]\d+['""]");
            var updatedSvg = regex.Replace(svg, $"width='{newWidth}'");
            regex = new Regex(@"height=['""]\d+['""]");
            updatedSvg = regex.Replace(updatedSvg, $"height='{newHeight}'");
            return updatedSvg;
        }
    }
}
