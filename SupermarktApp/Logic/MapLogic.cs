using System;
using System.Text;

public static class MapLogic
{   
    public static string green = "\x1b[38;2;20;110;21m";
    public static string bold = "\x1b[1m";
    public static string reset = "\x1b[0m";
    public static string mapString = $@"                               
                                                                       ╔═══════╦══╦══╦══╦══╦═════╗
                                        ╔════════════╦════════════╗    ║       │  │  │  │  │     ║
                                        ║            ║            ║    ║    ╔══╩══╩══╩══╩══╩═╗   ║
                                        ║            ║            ║    ╠════╣                ║═══╣
                                        ║            ║            ║    ║    ║                ║   ║
                                        ║            ║            ║    ╠════╣                ║═══╣
                                        ║            ║            ║    ║    ║                ║   ║
                                        ╠════════ WC ╩ WC ═════════════╩════╝                ║═══╣
                                        ║                                                    ║   ║
                                        ║                                                    ╚═══╣
                                        ║  ┌─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┐                           ║
                                        ║  │ │ │ │ │ │ │ │ │ │ │ │ │ │                           ║
                                        ║  └─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┘                           ║
                                        ║  ┌─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┐                           ║
                                        ║  │ │ │ │ │ │ │ │ │ │ │ │ │ │                           ║
                                        ║  └─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┘                  ▓▓▓▓▓▓▓▓▓║
                                        ║  ┌─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┬─┐                ▓▓▓▓▓▓▓▓▓▓▓║
                                        ║  │ │ │ │ │ │ │ │ │ │ │ │ │ │               ▓▓▓▓▓       ║
                                        ║  └─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┘               ▓▓▓▓        ║
                                        ║                                            ▓▓▓▓        ║
                                        ║          CHECKOUT                          ▓▓▓▓        ║
                                        ║    ░░   ░░   ░░   ░░   ░░                  ▓▓▓▓        ║
                                        ║    ░░   ░░   ░░   ░░   ░░                  ▓▓▓▓  ╔══╗  ║
                                        ║    ░░   ░░   ░░   ░░   ░░                  ▓▓▓▓  ║  ║  ║
                                        ╚═════════════════════════════ ENTER ══ EXIT ══════╩══╩══╝";
    public static string MapBuilder(int box)
    {
        StringBuilder outputMap = new StringBuilder(mapString);

        int position = 0;
        for (int i = 0; i < outputMap.Length; i++)
        {
            if (outputMap[i] == '│' && outputMap[i + 2] == '│' || outputMap[i] == '│' && outputMap[i + 3] == '│')
            {
                position++;
                if (position == box)
                {
                    outputMap.Remove(i + 1, 1);
                    outputMap.Insert(i + 1, green + bold + "X" + reset);
                    break;
                }
            }

        }
        
        return outputMap.ToString();
    }
}