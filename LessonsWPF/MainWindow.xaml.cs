using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace LessonsWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string baseFile;
        private string currentFile;
        private bool isCreateFile = true;
        public MainWindow()
        {
            InitializeComponent();
            baseFile = @"c:\temp\Text.txt";
            Title = String.Format($"{System.IO.Path.GetFileNameWithoutExtension(baseFile)} - Notebook");
        }
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            //Вихід із блокнота, саме легше що могло бути, а вот решта просто жесть =)
            Close();
        }

        private void MenuCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /* MSDN: десь години дві не міг розібратись зі створенням нового файла. Ще потоки не розбирав,
                 * тому на цьому трохи застряг, але врешті решт просто треба було додати using{} і все запрвцювало.
                 * https://docs.microsoft.com/ru-ru/dotnet/api/system.io.file.create?view=netframework-4.8
                 * https://docs.microsoft.com/en-us/dotnet/api/system.io.file?view=netframework-4.8
                 * https://docs.microsoft.com/en-us/dotnet/api/system.io.file.create?view=netframework-4.8#System_IO_File_Create_System_String_
                 * https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader?view=netcore-3.1
                 */
                using (File.Create(baseFile))                                           // Створюємо файл.
                {
                    isCreateFile = true;
                    Title = String.Format($"{System.IO.Path.GetFileNameWithoutExtension(baseFile)} - Notebook");
                }                  
                using (StreamReader sr = File.OpenText(baseFile))                       // Відкриваємо файл.
                {                    
                    textBox.Text = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            /* OpenFileDialog Class відкриття 
             * https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.openfiledialog?view=netframework-4.8
             * https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-read-text-from-a-file
             * https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader?view=netframework-4.8
             * Все брав звідси.MSDN потужний сайт, але є багато складних прикладів в яких деколи складно розібратись.
             */
            OpenFileDialog dialOpen = new OpenFileDialog();                         //Налаштування діалогового вікна відкритого файлу.
            dialOpen.FileName = "Document";                                         // Назва файлу за замовчуванням.
            dialOpen.DefaultExt = ".txt";                                           // Розширення файлу за замовчуванням.
            dialOpen.Filter = "Text documents (.txt)|*.txt";                        // Фільтр файлів за розширенням.

            // Показати діалогове вікно відкритого файлу
            bool? result = dialOpen.ShowDialog();
            // Обробляти результати діалогового вікна відкритого файлу
            if (result == true)
            {
                StreamReader fileRead = new StreamReader(dialOpen.FileName);        // Відкриваємо текстовий файл за допомогою зчитувача потоків.                
                textBox.Text = fileRead.ReadToEnd();                                // Читаєм текстовий документ і виводимо його в textBox.
                Title = String.Format($"{System.IO.Path.GetFileNameWithoutExtension(dialOpen.FileName)} - Notebook");
                currentFile = dialOpen.FileName;
                isCreateFile = false;
                fileRead.Close();
            }
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            /* MSDN:
             * https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter?view=netcore-3.1
             * https://docs.microsoft.com/en-us/dotnet/api/system.io.file?view=netframework-4.8
             * https://docs.microsoft.com/en-us/dotnet/api/system.io.fileinfo?view=netframework-4.8
             * спершу не вдавалось реалізувати цей метод здбереження, два дні перекурив, і ще раз переосмислив та перечитавши літератури
             * реалізував робочий варіант :D
             */
            try
            {
                string copyText = textBox.Text;                                     // Робимо копію тексту
                if(isCreateFile)
                {
                    using (StreamWriter streamWriter = new StreamWriter(baseFile))      // Перезаписуємо поточний файл, коли ми його тільки створили
                    {
                        streamWriter.Write(copyText);
                        streamWriter.Close();
                    }
                }
                else
                {
                    using (StreamWriter streamWriter = new StreamWriter(currentFile)) // Перезаписуємо поточний файл, який ми відкрили
                    {
                        streamWriter.Write(copyText);
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            /* MSDN:
             * https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.savefiledialog?view=netframework-4.8
             * https://docs.microsoft.com/en-us/dotnet/api/system.io.file.writealltext?view=netframework-4.8#System_IO_File_WriteAllText_System_String_System_String_
             * https://docs.microsoft.com/en-us/dotnet/api/system.io.file?view=netframework-4.8
             */
            SaveFileDialog dialogSaveAs = new SaveFileDialog();                     // Налаштування діалогового вікна збереження файлу.
            dialogSaveAs.FileName = "TextDoc";                                      // Назва файлу за замовчуванням.
            dialogSaveAs.DefaultExt = ".txt";                                       // Розширення файлу за замовчуванням.
            dialogSaveAs.Filter = "Text documents (.txt)|*.txt";                    // Фільтр файлів за розширенням.

            // Показати діалогове вікно збереження файлу.
            bool? result = dialogSaveAs.ShowDialog();
            // Обробляти результати діалогового вікна збереження файлу.
            if (result == true)
            {
                File.WriteAllText(dialogSaveAs.FileName, textBox.Text);              // Зберегти документ.                
            }
        }

        private void MenuPagaSetting_Click(object sender, RoutedEventArgs e)
        {
            /* 
             * Функціонал вікна налаштуваннь.
             * поки чистий...
             */
            WindowSetting windowSetting = new WindowSetting();
            windowSetting.Show();                                                   // Виводимо вікно налаштувань.
        }

        private void MenuDateTime_Click(object sender, RoutedEventArgs e)
        {
            /* Структура DateTime - Робота з датами і часом
             * Аналог функіоналу із стандартного блокноту
             * https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.primitives.textboxbase.appendtext?view=netframework-4.8#System_Windows_Controls_Primitives_TextBoxBase_AppendText_System_String_
             * https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.textbox?view=netframework-4.8
             * - працює трохи не так як в оригіналі.(textBox.Text = dateTime.ToString("HH:mm dd.MM.yyyy");) ПОФІКСИВ!
             * Тепер то що треба
             */
            DateTime dateTime = DateTime.Now;                                       // Встановлюємо поточний час.
            textBox.AppendText(dateTime.ToString("HH:mm dd.MM.yyyy"));              // Виводимо поточний час в textBox.


        }
    }
}
