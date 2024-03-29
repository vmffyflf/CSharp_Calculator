﻿using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;

namespace Calculater
{
    public partial class MainPage : Form
    {
        string res;
        Calculator nav;
        public MainPage()
        {
            InitializeComponent();
        }

        private void NavCalc(object sender, EventArgs e)
        {
            nav = new Calculator();
            nav.Show();
            nav.ResultCalculated += ResultMap;

        }

        private void ResultMap(object sender, ResultEventArgs e)
        {
            res = e.Result;
            ResultValue.Text = "결과값 : " + res;

        }

        private void Text_Click(object sender, EventArgs e)
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // 출력할 파일 경로
            string Text_Path = Path.Combine(filePath, "output.txt");
            string dataToWrite = res; //출력할 값

            try
            {
                // 파일이 이미 존재하는지 확인
                bool fileExists = File.Exists(Text_Path);

                // StreamWriter를 사용하여 파일에 텍스트를 추가 또는 쓰기
                using (StreamWriter writer = new StreamWriter(Text_Path, fileExists))
                {
                    // 파일이 이미 존재하는 경우 새로운 줄로 추가
                    if (fileExists)
                    {
                        writer.WriteLine(dataToWrite);
                        Console.WriteLine("Text Write new line");
                    }
                    // 파일이 존재하지 않는 경우 새로운 파일을 생성하고 데이터를 씀
                    else
                    {
                        writer.Write(dataToWrite);
                        Console.WriteLine("Create a new File");
                    }
                }

                Console.WriteLine("Text has been written to the file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void Excel_Click(object sender, EventArgs e)
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // 출력할 파일 경로
            string Excel_Path = Path.Combine(filePath, "output.xlsx");
            string dataToWrite = res;

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                bool fileExists = File.Exists(Excel_Path);
                excelApp.DisplayAlerts = false;

                if (fileExists)
                {
                    workbook = excelApp.Workbooks.Open(Excel_Path);
                    worksheet = workbook.Sheets[1];
                    int lastRow = worksheet.Cells[worksheet.Rows.Count, 1].End[Microsoft.Office.Interop.Excel.XlDirection.xlUp].Row + 1;
                    worksheet.Cells[lastRow, 1] = dataToWrite;
                    Console.WriteLine("ok");
                }
                else
                {
                    workbook = excelApp.Workbooks.Add();
                    worksheet = workbook.Sheets[1];
                    worksheet.Cells[1, 1] = dataToWrite;
                    Console.WriteLine("not ok");
                }
                workbook.SaveAs(Excel_Path);
                workbook.Close();
                excelApp.Quit();

                Console.WriteLine("Data has been written to the Excel file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                excelApp.Quit();
            }
            finally
            {
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(excelApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Console.WriteLine("Data has been written to the Excel file successfully.");
            }
        }

        private void DataBase_Click(object sender, EventArgs e)
        {
            string dataToWrite = res;
            string connStr = ConfigurationManager.AppSettings["DbConnectionString"];
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                Console.WriteLine("Connecting to MariaDB...");
                conn.Open();

                string sql = "INSERT INTO tbl (Result) VALUES (@dataToWrite)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@dataToWrite", dataToWrite);
                cmd.ExecuteNonQuery();
                Console.WriteLine("The calculator result has been stored successfully.");
            } catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine("Done.");

        }

        private void ReadList_Click(object sender, EventArgs e)
        {
            DataList list = new DataList(); 
            list.Show();

        }
    }
}
