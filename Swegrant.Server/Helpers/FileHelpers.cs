using Export.XLS;
using Swegrant.Shared.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Swegrant.Server.Helpers
{
    public class FileHelpers
    {

        public static string[] GetFileList(Mode mode, int scene = 0)
        {
            DirectoryInfo mediaDirectory = null;
            if (mode == Mode.Video)
            {
                mediaDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\Video");
            }
            else if (mode == Mode.Theater)
            {
                mediaDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\Theater");
            }
            
            if (mediaDirectory != null)
            {
                FileInfo[] files = mediaDirectory.GetFiles("*.*", SearchOption.AllDirectories);
                if (scene == 0)
                    return files.Select( c=> c.Name ).ToArray();
                else
                    return files.Where( c => c.Name.Contains(scene.ToString("00"))).Select( c => c.Name ).ToArray();    
            }
            return null;
        }

        public static bool ExportExcelQuestionnaire(ObservableCollection<SubmitQuestion> collection)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                DirectoryInfo exportDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\Export");
                if (!exportDirectory.Exists)
                {
                    Directory.CreateDirectory(exportDirectory.FullName);
                }

                SubmitQuestion[] array = collection.ToArray();

                ExcelDocument document = new ExcelDocument();
                document.UserName = "Saman";
                document.CodePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;

                document.Cell(0, 0).Value = "Id";
                document.Cell(0, 1).Value = "Username";
                document.Cell(0, 2).Value = "Question";
                document.Cell(0, 3).Value = "Answer";

                for (int i = 0; i < array.Length; i++)
                {

                    document.Cell(i + 1, 0).Value = array[i].Id;
                    document.Cell(i + 1, 1).Value = array[i].Username;
                    document.Cell(i + 1, 2).Value = array[i].Title;
                    document.Cell(i + 1, 3).Value = array[i].Value;
                }
                string fileName = $"{exportDirectory}\\Questionnaire_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm")}.xls";

                FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate);

                document.Save(stream);
                stream.Close();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static bool ExportExcelUserStatus(ObservableCollection<SubmitUserStatus> collection)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                DirectoryInfo exportDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\Export");
                if (!exportDirectory.Exists)
                {
                    Directory.CreateDirectory(exportDirectory.FullName);
                }

                SubmitUserStatus[] array = collection.ToArray();

                ExcelDocument document = new ExcelDocument();
                document.UserName = "Saman";
                document.CodePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;

                document.Cell(0, 0).Value = "Id";
                document.Cell(0, 1).Value = "Username";
                document.Cell(0, 2).Value = "Event";
                document.Cell(0, 3).Value = "Value";
                document.Cell(0, 4).Value = "Time";

                for (int i = 0; i < array.Length; i++)
                {

                    document.Cell(i+1, 0).Value = array[i].Id;
                    document.Cell(i+1, 1).Value = array[i].Username;
                    document.Cell(i+1, 2).Value = array[i].Event.ToString();
                    document.Cell(i+1, 3).Value = array[i].Value;
                    document.Cell(i+1, 4).Value = array[i].Time.ToString("yyyy-MM-dd HH:mm:ss");
                }
                string fileName = $"{exportDirectory}\\UserStatus_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm")}.xls";

                FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate);

                document.Save(stream);
                stream.Close();



                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
