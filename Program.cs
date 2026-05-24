using System;
using System.IO;
using System.Text.RegularExpressions;

namespace EggyPartyCopyMapTextProcessor
{
    class Program
    {
        public static void Main()
        {
            // 输入文本格式：复制打开【蛋仔派对】，游玩地图《地图名》，地图码1234-5678。
            Console.Write("输入从游戏中复制的文本：");
            string inputText = Console.ReadLine();

            if (string.IsNullOrEmpty(inputText))
            {
                return;
            }

            try
            {
                // 提取地图名（《...》之间的内容）
                string mapName = null;
                Match mapNameMatch = Regex.Match(inputText, @"《([^》]+)》");
                if (mapNameMatch.Success)
                {
                    mapName = mapNameMatch.Groups[1].Value.Trim();
                }
                else
                {
                    throw new Exception("未找到地图名（未匹配到《地图名》）");
                }

                // 提取地图码（数字-数字格式，或连续数字，此处按示例格式）
                string mapCode = null;
                Match mapCodeMatch = Regex.Match(inputText, @"地图码(\d+[-]?\d+)");
                if (mapCodeMatch.Success)
                {
                    mapCode = mapCodeMatch.Groups[1].Value.Trim();
                }
                else
                {
                    // 尝试更宽松的匹配：连续数字或数字+横杠+数字
                    Match fallbackMatch = Regex.Match(inputText, @"(\d{4,}-\d+)");
                    if (fallbackMatch.Success)
                    {
                        mapCode = fallbackMatch.Groups[1].Value;
                    }
                    else
                    {
                        throw new Exception("未找到地图码");
                    }
                }

                // 生成内容
                string outputContent = $"【蛋仔派对】地图《{mapName}》试玩\n\n相关游戏：蛋仔派对\n地图名：{mapName}\n地图码：{mapCode}\n(EggyPartyCopyMapTextProcessor Generate)";

                // 写入文件（程序目录下，文件名包含地图名以防重复）
                string safeMapName = string.Join("_", mapName.Split(Path.GetInvalidFileNameChars()));
                string fileName = $"EggyPartyMap_{safeMapName}.txt";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                File.WriteAllText(filePath, outputContent, System.Text.Encoding.UTF8);

                Console.WriteLine($"文件已生成：{filePath}");
                Console.WriteLine("内容如下：");
                Console.WriteLine(outputContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理出错：{ex.Message}");
                // 可记录日志，此处简单输出到控制台
                File.WriteAllText("error_log.txt", $"{DateTime.Now}: {ex.ToString()}", System.Text.Encoding.UTF8);
            }
        }
    }
}