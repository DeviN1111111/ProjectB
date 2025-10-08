using Spectre.Console;
using System.Text;
public static class Logo
{
  private static readonly string asciiLogo = @"                                                 
                                                                                                    
                                                      ▓▓▓▓▓▓▓▓                                      
                                                     ▓▓▓▓ ▓▓ ▓▓                                     
                         ▓          ▓▓▓▓            ▓▓▓▓▓  ▓ ▓▓                                     
                       ▓▓▓▓    ▓▓▓ ▓▓▓▓        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                                     
                       ▓▓▓▓▓   ▒▒▒ ▓▓▓▓   ▒▒   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                                 
                        ▓▓▓▓ ▒▒▒▒▒▓▓▓▓▓ ▒▒▒▒  ▓▓▓▓▓▒▒▒▒░░▒▒▒▒▒▒▒▒▒▒                                 
                       ▓▓▓▓▓▒▒▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒ ▓▓▓▓▓▒▒▒░▒▒░▒▒░░▒▒▒▒▓                                 
         ▓▓▓▓▓▓▓▓████  ▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓▓▒▒▒▒░░▒▒░▒▒▒▒▒▒▓                                 
         ▓▓▓▓▓▓▓▓█████ ▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓▒▒▒▒▒▒░░▒▒░▒▒▒▒▒                                 
                   ████ ▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▒▒▒▒▒░▒▒▒░▒▒░▒▒▓                                 
                   ███████████████████████████████████████████████████                              
                    ███████████████████████████████████████████████████                             
                     ████     ████      █████     █████     ████  █████                             
                      ████    █████      ████      ████      ████ ████                              
                      ████     ████      █████      ████     █████████                              
                       ████     ████      ████      █████     ███████                               
                       █████     ████      ████      ████     ███████                               
                        ████████████████████████████████████████████                                
                         ███████████████████████████████████████████                                
                         █████     ████      ████      ████     ████                                
                          █████    █████      ████      ████   ████                                 
                           ████     ████       ███      ████   ████                                 
                           █████     ████      ████      ████ █████                                 
                            ██████████████████████████████████████                                  
                             █████████████████████████████████████                                  
                             ███████████████████████████████████                                    
                            ████                                                                    
                           █████   ████                █████                                        
                           ████ ██████████          ██████████                                      
                          █████████▓▓▓▓████████████████▓▓▓▓████                                     
                          ████████▓▓▓▓▓▓██████████████▓▓▓▓▓▓███                                     
                           ████████▓▓▓▓████████████████▓▓▓▓████                                     
                                ██████████          ██████████                                      
                                  ██████              ██████                                        
";
  public static void Show()
  {
      var width = AnsiConsole.Profile.Width > 0 ? AnsiConsole.Profile.Width : Console.WindowWidth;

      var centered = Center(asciiLogo, width);
      var color = new Color(0x1B, 0x98, 0xE0);
      AnsiConsole.Write(new Text(centered, new Style(color)));
  }
  private static string Center(string text, int targetWidth)
  {
    var lines = text.Replace("\r", "").Split('\n');
    var blockWidth = lines.Max(l => l.Length);

    var leftPad = Math.Max(0, (targetWidth - blockWidth) / 2);
    var pad = new string(' ', leftPad);

    var sb = new StringBuilder(text.Length + leftPad * lines.Length);
    foreach (var line in lines)
    {
      var trimmed = line.TrimEnd();
      if (trimmed.Length == 0)
      sb.AppendLine();
      else
      sb.Append(pad).AppendLine(trimmed);
    }
    return sb.ToString();
  }
}
