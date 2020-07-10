using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
namespace pdfStamp
{
    public partial class Form1 : Form
    {

        string filePath = string.Empty;
        string filePathOut = string.Empty;
        string numberInStamp = string.Empty;
        string currentDate = string.Empty;
        string fileName = string.Empty;
        string fileNameOut = string.Empty;

        
        Dictionary<string, string> Month = new Dictionary<string, string>
        {
            {"1", "января"},
            {"2", "февраля"},
            {"3", "марта"},
            {"4", "апреля"},
            {"5", "мая"},
            {"6", "июня"},
            {"7", "июля"},
            {"8", "августа"},
            {"9", "сентября"},
            {"10", "октября"},
            {"11", "ноября"},
            {"12", "декабря"},
        };
    

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label4.ForeColor = Color.Red;
            label4.Text = "Идет выполнение";
            numberInStamp = numStamp.Text;
            
            
            //if (outDerectory.Text == "")
            //{
            //    filePathOut = filePath.Remove(filePath.LastIndexOf('\\'));
            //}

            button1.Enabled = false;
            label4.Visible = true;

            
            DateTime dt = dateTimePicker1.Value;//Получение текущей даты 
            string month = string.Empty;
            if(dt.Month < 10)
            {
                month = "0" + dt.Month.ToString();
            }
            else
            {
                month = dt.Month.ToString();
            }

            //Устанавливаем номер и дату в печать
            // old "C:\\distr\\blueStampMini.png -fill Black -pointsize 18 -draw \"gravity SouthEast text 100,98  '01/101-" + numberInStamp + "'\" -draw \"gravity SouthEast text 255,72 '" + dt.Day.ToString() + "'\" -draw \"gravity SouthEast text 155,72 '" + month + "'\" -draw \"gravity SouthEast text 60,72 '" + dt.Year%2000 + "'\" C:\\distr\\completedStamp.png";
            string convStampArgs = "C:\\distr\\blueStampMini.png +contrast -fill Black -pointsize 14 -weight Bold -draw \"gravity SouthEast text 50,60  '01/101-" + numberInStamp + "'\" -draw \"gravity SouthEast text 150,42 '" + dt.Day.ToString() + "'\" -draw \"gravity SouthEast text 95,42 '" + month + "'\" -draw \"gravity SouthEast text 35,42 '" + dt.Year%2000 + "'\" C:\\distr\\completedStamp.png";
            Process convetrStamp = new Process();
            convetrStamp.StartInfo.FileName = "C:\\distr\\im\\convert.exe";
            convetrStamp.StartInfo.Arguments = convStampArgs;
            convetrStamp.StartInfo.UseShellExecute = false;
            convetrStamp.StartInfo.RedirectStandardOutput = true;
            convetrStamp.StartInfo.CreateNoWindow = true;
            convetrStamp.Start();
            convetrStamp.WaitForExit();
            if (convetrStamp.ExitCode !=0)
            {
                int errorCod = convetrStamp.ExitCode;
                MessageBox.Show("convetrStamp"+errorCod.ToString());
            }
            convetrStamp.Close();

            string convStampOnPDF = "-composite -density  120 -quality 200 " + filePath+ "[0]" + " C:\\distr\\completedStamp.png -gravity southeast -geometry +100+100 c:\\distr\\" + "stampedPage.pdf";
            Process stampOnPDF = new Process();
            stampOnPDF.StartInfo.FileName = "C:\\distr\\im\\convert.exe";
            stampOnPDF.StartInfo.Arguments = convStampOnPDF;
            stampOnPDF.StartInfo.UseShellExecute = false;
            stampOnPDF.StartInfo.RedirectStandardOutput = true;
            stampOnPDF.StartInfo.CreateNoWindow = true;
            stampOnPDF.Start();
            stampOnPDF.WaitForExit();
            if (stampOnPDF.ExitCode != 0)
            {
                int errorCod = stampOnPDF.ExitCode;
                MessageBox.Show("stampOnPDF"+errorCod.ToString());
            }
            stampOnPDF.Close();

            string convFullPdf = "--overlay  c:\\distr\\stampedPage.pdf" + " --from=1 --to=1 -- " + filePath + " " +Properties.Settings.Default.defaultPathOut + "\\"+ "Вх_№_01_101_" + Properties.Settings.Default.stumpNum + ".pdf";

            Process fullPdf = new Process();
            fullPdf.StartInfo.FileName = "C:\\distr\\qpdf\\bin\\qpdf.exe";
            fullPdf.StartInfo.Arguments = convFullPdf;
            fullPdf.StartInfo.UseShellExecute = false;
            fullPdf.StartInfo.RedirectStandardOutput = true;
            fullPdf.StartInfo.CreateNoWindow = true;
            fullPdf.Start();
            fullPdf.WaitForExit();
            if (fullPdf.ExitCode != 0)
            {
                int errorCod = fullPdf.ExitCode;
                MessageBox.Show("fullPdf" + errorCod.ToString());
            }
            fullPdf.Close();


            label4.ForeColor = Color.Green;
            label4.Text = "Готово";

            //System.IO.File.Delete("C:\\distr\\completedStamp.png");
            //System.IO.File.Delete("C:\\distr\\stampedPage.pdf");
            button1.Enabled = true;
            Properties.Settings.Default.stumpNum++;
            Properties.Settings.Default.Save();
            numStamp.Text = Properties.Settings.Default.stumpNum.ToString();
            nameFile.Text = "Вх_№_01_101_" + Properties.Settings.Default.stumpNum.ToString();

        }

        //Вызов окна выбора файла
        private void button2_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }
            selectFile.Text = filePath;
            //filePathOut = filePath.Remove(filePath.LastIndexOf('\\'));
            //outDerectory.Text = filePathOut;
            //Активируем кнопку "старт", при запуске неактивна
            if (filePath != "")
            {
                button1.Enabled = true;
            }
            fileName = Path.GetFileName(filePath);
            nameFile.Text = "Вх_№_01_101_" + Properties.Settings.Default.stumpNum;

        }

        //Окно выбора места сохранения сайта
        private void SaveFileBtn_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePathOut = dialog.SelectedPath;
                }
            outDerectory.Text = filePathOut;
            Properties.Settings.Default.defaultPathOut = filePathOut;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            currentDate = date.ToShortDateString();
            //кнопка старт выключена по умолчанию 
            button1.Enabled = false;
            label4.Visible = false;
            //устанавливаем номер печати из settings
            numStamp.Text = Properties.Settings.Default.stumpNum.ToString();
            outDerectory.Text = Properties.Settings.Default.defaultPathOut.ToString();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && e.Shift)
            {
                settings settingsForm = new settings();
                settingsForm.Show();
                MessageBox.Show("JR");
            }

        }
        
        private void numStamp_TextChanged(object sender, EventArgs e)
        {
            nameFile.Clear();
            nameFile.Text = "Вх_№_01_101_"+numStamp.Text.Replace(" ","_").Replace("/","_");
            fileNameOut = nameFile.Text;
            fileNameOut = fileNameOut.Replace(" ", "_").Replace("/", "_");

            if (numStamp.Text != "")
            {
               Properties.Settings.Default.stumpNum = Convert.ToInt32(numStamp.Text);
            }
    
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
           Properties.Settings.Default.stumpNum++;
           numStamp.Text = Properties.Settings.Default.stumpNum.ToString();
           Properties.Settings.Default.Save();
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.stumpNum !=0)
            {
                Properties.Settings.Default.stumpNum--;
                numStamp.Text = Properties.Settings.Default.stumpNum.ToString();
                Properties.Settings.Default.Save();
            }
        }
    }
}
