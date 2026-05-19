namespace Tools
{

    //Â·¾¶Ïà¹Ø
    public class Path
    {
        public static string GetDirectory(string path)
        {
            return path.Substring(0, path.LastIndexOf('/'));
        }

        public static string GetName(string path)
        {
            return path.Substring(path.LastIndexOf('/') + 1);
        }

        public static string GetPath(params string[] strs)
        {
            string path = "";
            
            foreach(string str in strs)
            {
                path += str + "/";
            }
            return path.Substring(0, path.LastIndexOf('/'));
        }
    }
}
