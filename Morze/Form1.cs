using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BaseNcoding;
using System.Security.Cryptography;


namespace Morze
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private byte[] ComputeMD5Checksum(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                //string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                return checkSum;
            }
        }

        private string FormatToRG(string data)
        {
            string inputText = "";
            int schet = 0;
            int perenos = 0;
            // Создаем форматированую строку для записи в файл
            foreach (char bits in data)
            {

                if (schet == 5)         // Добавлять пробел после 5 символов
                {
                    inputText += ' ';
                    schet = 0;
                    perenos++;
                    if (perenos == 10)  // Добавлять перенос после 10 групп по 5 символов
                    {
                        inputText += '\n';
                        perenos = 0;
                    }
                }
                inputText += bits;
                schet++;
            }
            return inputText;
        }

        public byte[] DecBytes;
        List<byte> Bytes = new List<byte>();
        List<byte> WrBytes = new List<byte>();
        byte lowMask = 0x0F;    // Маска для младших бит
        byte hiMask = 0xF0;     // Маска для старших бит
        Dictionary<byte, char> Table = new Dictionary<byte, char>();
        Dictionary<char, byte> InvTable = new Dictionary<char, byte>();


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            Stream FStream = null;
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "ENC files (*.ENC)|*.ENC";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            
            // Составляем таблицы для прямого и обратного преобразования
            byte cod = 0;
            char sumbol = 'А';            
            for (int i = 0; i < 32; i++)
            {
                Table.Add(cod, sumbol);     // Ключ - бит, значение - символ
                InvTable.Add(sumbol, cod);  // Ключ - символ, значение - бит
                cod++;
                sumbol++;
            }



            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((FStream = openFileDialog1.OpenFile()) != null)
                    {
                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message); 
                }
            }

            byte[] md5Hash = ComputeMD5Checksum(openFileDialog1.FileName);            
            BaseN Hachcod = new BaseN();
            string strHash = Hachcod.Encode(md5Hash);
            FStream = openFileDialog1.OpenFile();
            var hvost = 5 - FStream.Length % 5;
            DecBytes = new byte[FStream.Length];
            BinaryReader BinReader = new BinaryReader(FStream);
            while (FStream.Position < FStream.Length)
            {
                ulong ind = (ulong)FStream.Position;
                DecBytes[FStream.Position] = (byte)(BinReader.ReadByte());

            }
            /*
            // Заполняем хвосты нулями и добавляем в последний байт длину хвоста
            if (hvost == 0)
            {
                DecBytes = new byte[FStream.Length + 5];
                DecBytes[DecBytes.Count()-1] = 0;
                for (var i = FStream.Length; i > DecBytes.Count() - 1; i++)
                {
                    DecBytes[i] = 0;
                }
            }
            else
            {
                DecBytes = new byte[FStream.Length + hvost];
                DecBytes[DecBytes.Count() - 1] = (byte)hvost;
                for (var i = FStream.Length; i > DecBytes.Count() - 1; i++)
                {
                    DecBytes[i] = 0;
                }
            }
            BinaryReader BinReader = new BinaryReader(FStream);

            while (FStream.Position < FStream.Length)
            {
                ulong ind = (ulong)FStream.Position;
                DecBytes[FStream.Position] = (byte)(BinReader.ReadByte());
                
            }
            ulong buffer;   // Буфер для 40 бит            
            for (var i = 0; i < DecBytes.Count(); i += 5)
            {
                buffer = 0;
                for (int j = 0; j < 5; j++) // Заполняем буфер 
                {
                    buffer = (buffer << 8) + DecBytes[i + j];
                }
                byte[] byteBuff = new byte[8];
                for (int j = 7; j >= 0; j--)    // Заполняем буферный массив
                {
                    byteBuff[j] = (byte)(buffer & 31);
                    buffer >>= 5;
                }
                for (int j = 0; j < 8; j++) 
                {
                    Bytes.Add(byteBuff[j]);
                }

            }*/
            // Прямое преобразование (в радиограмму)
            /*while (FStream.Position < FStream.Length)
            {
                ulong ind = (ulong)FStream.Position;
                DecBytes[FStream.Position] = (byte)(BinReader.ReadByte());
                byte lowBit = (byte)(DecBytes[ind] & lowMask);      // Младшие 4 бита байта
                byte hiBit = (byte)((DecBytes[ind] & hiMask) >> 4); // Старшие 4 бита байта
                // Заполняем список полубайтами
                Bytes.Add(hiBit);
                Bytes.Add(lowBit);
            }*/
            var refDec = DecBytes;
            BaseN triOd = new BaseN();
            string alf = strHash + triOd.Encode(refDec);
            FStream.Close();
            string inputText = FormatToRG(alf);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            for (int i = 0; i < 100; i++)
            {
                progressBar1.Value++;
                System.Threading.Thread.Sleep(20);

            }

            Stream FWStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((FWStream = saveFileDialog1.OpenFile()) != null)
                {
                    UTF8Encoding UTF8WithPreamble = new UTF8Encoding(true);
                    StreamWriter BinWriter = new StreamWriter(FWStream, UTF8WithPreamble);
                    BinWriter.Write(inputText);
                    BinWriter.Close();
                    FWStream.Close();
                }
            }
            //progressBar1.Value = 0;
            Table.Clear();
            InvTable.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            Stream FStream = null;
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;


            // Составляем таблицы для прямого и обратного преобразования
            byte cod = 0;
            char sumbol = 'А';            
            for (int i = 0; i < 16; i++)
            {
                Table.Add(cod, sumbol);     // Ключ - бит, значение - символ
                InvTable.Add(sumbol, cod);  // Ключ - символ, значение - бит
                cod++;
                sumbol++;
            }


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((FStream = openFileDialog1.OpenFile()) != null)
                    {

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            
            Stream fs = openFileDialog1.OpenFile();            
            byte[] DecBytes = new byte[fs.Length-3];
            BinaryReader BinReader = new BinaryReader(fs);
            while (fs.Position < fs.Length)
            {
                if (fs.Position < 3)
                {
                    BinReader.ReadByte();
                    continue;
                }
                DecBytes[fs.Position-3] = (byte)(BinReader.ReadByte());

            }
            BinReader.Close();
            fs.Close();



            //Получаем строку и удаляем из нее символы форматирования
            string Log_txt = Encoding.UTF8.GetString(DecBytes, 0, DecBytes.Length);
            Log_txt = Log_txt.Replace("\n", "");
            Log_txt = Log_txt.Replace(" ", "");

            string strHash = Log_txt.Substring(0,26);
            var refDec = Log_txt.Substring(26,Log_txt.Length-26);
            BaseN triOd = new BaseN();
            byte[] alf = triOd.Decode(refDec);

            /*// Обратное преобразование
            foreach (char letter in Log_txt)
            {
                Bytes.Add(InvTable[letter]);    // Создаем список полубайт
            }
            byte[] WrtBytes = new byte[Bytes.Count()/2];
            for (int i = 0; i < Bytes.Count(); i += 2)
            {
                byte buf = (byte)((Bytes[i] << 4) + (Bytes[i+1]));  // Складываем два полубайта в байт
                WrtBytes[i / 2] = buf;
            }
            */

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            for (int i = 0; i < 100; i++)
            {
                progressBar1.Value++;
                System.Threading.Thread.Sleep(12);

            }
            System.Threading.Thread.Sleep(400);

            Stream FWStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "ENC files (*.ENC)|*.ENC";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((FWStream = saveFileDialog1.OpenFile()) != null)              
                {
                    BinaryWriter BinWriter = new BinaryWriter(FWStream);
                    BinWriter.Write(alf);
                    BinWriter.Close();
                    FWStream.Close();
                }
            }

            byte[] md5Hash = ComputeMD5Checksum(saveFileDialog1.FileName);
            BaseN Hachcod = new BaseN();
            if (strHash != Hachcod.Encode(md5Hash))
                MessageBox.Show("Файл поврежден. Хэш суммы не равны!!!"); 
            //progressBar1.Value = 0;
            Table.Clear();
            InvTable.Clear();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
