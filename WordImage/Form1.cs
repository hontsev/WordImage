using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Mozilla.NUniversalCharDet;
using JiebaNet.Segmenter.PosSeg;
using JiebaNet.Segmenter;

namespace WordImage
{
    public enum Type
    {
        Chars,Words,WordImage
    }
    public partial class Form1 : Form
    {
        string dropchar = " \r\n,.'\"<>=_-+，。、·“”‘’《》——！？…（）~；：;:~!@#$%^&*()[]{}/";
        Pair[] cutWordList;
        string fileinfo;
        List<Word> word;
        List<Word> word2;
        List<Line> lines;
        string[] wordArray;
        int wordnum;
        Bitmap wordImage;
        Type type;

        int side = 30;
        int wordsize = 10;
        int lastn;
        Point lastp;
        bool isFirst = false;
        bool newBegin = false;

        delegate void sendIntDelegate(int n);
        delegate void sendPointDelegate(Point p);
        delegate void sendStringDelegate(string str);

        static Color lineColor1 = Color.FromArgb(5, 0, 50, 100);
        static Color textColor1 = Color.FromArgb(200, 0, 0, 100);
        static Pen linePen1 = new Pen(lineColor1);
        static Font textFont1 = new Font("SimSun", 8, FontStyle.Regular);
        static SolidBrush textBrush1 = new SolidBrush(textColor1);
        static SolidBrush textBrush2 = new SolidBrush(lineColor1);
        Graphics g;

        public Form1()
        {
            InitializeComponent();
            type = Type.WordImage;
        }

        private void paintNewLine(int n)
        {
            if (pictureBox1.InvokeRequired)
            {
                sendIntDelegate paintEvent = new sendIntDelegate(paintNewLine);
                Invoke(paintEvent,(object)n);
            }
            else
            {
                Point p1;
                Point p2;

                p1 = new Point(side+10 , side + lastn * wordsize + wordsize / 2);
                p2 = new Point(wordImage.Width - side, side + n * wordsize + wordsize / 2);

                g.DrawLine(linePen1, p1, p2);

                pictureBox1.Image = wordImage;
                pictureBox1.Refresh();
            }
        }

        private void addLine(Point p1, Point p2)
        {
            foreach (var l in lines)
            {
                if ((l.x1 == p1.X && l.y1 == p1.Y && l.x2 == p2.X && l.y2 == p2.Y) 
                    || (l.x1 == p2.X && l.y1 == p2.Y && l.x2 == p1.X && l.y2 == p1.Y))
                {
                    //l.
                }
            }
        }

        private void paintNewLine2(Point nowp)
        {
            if (pictureBox1.InvokeRequired)
            {
                sendPointDelegate paintEvent = new sendPointDelegate(paintNewLine2);
                Invoke(paintEvent, (object)nowp);
            }
            else
            {
                
                Point p1;
                Point p2;
                p1 = new Point(side + lastp.X * wordsize + wordsize / 2, side + lastp.Y * wordsize + wordsize / 2);
                p2 = new Point(side + nowp.X * wordsize + wordsize / 2, side + nowp.Y * wordsize + wordsize / 2);

                string str1 = wordArray[lastp.X] + wordArray[lastp.Y];
                string str2 = wordArray[nowp.X] + wordArray[nowp.Y];

                g.DrawString(str1,textFont1,textBrush2,p1);
                g.DrawString(str2,textFont1,textBrush2,p2);

                g.DrawLine(linePen1, p1, p2);

                pictureBox1.Image = wordImage;
                pictureBox1.Refresh();
            }
        }

        private void printInfo(string str)
        {
            if (label1.InvokeRequired)
            {
                sendStringDelegate printEvent = new sendStringDelegate(printInfo);
                Invoke(printEvent, (object)str);
            }
            else
            {
                label1.Text = str;
            }
        }

        private int getWordNum(char tword)
        {
            int res = -1;
            //if (dropchar.IndexOf(tword) >= 0) return res;
            for (int i = 0; i < wordArray.Length; i++)
            {
                if (wordArray[i] == tword.ToString())
                {
                    res = i;
                    break;
                }
            }
            return res;
        }

        private int getWordNum(Pair tword)
        {
            int res = -1;
            //if (dropchar.IndexOf(tword) >= 0) return res;
            for (int i = 0; i < wordArray.Length; i++)
            {
                if (wordArray[i] == tword.Word)
                {
                    res = i;
                    break;
                }
            }
            return res;
        }

        private void buildWordArray()
        {
            try
            {
                if (wordnum > 100) wordnum = 70;
                wordArray = new string[wordnum];
                word.Sort();


                //for (int i = 0; i < wordnum; i++)
                //{
                //    wordArray[i] = word[i].word;
                //}


                
                bool left = false;
                int n = 0;
                foreach (var w in word)
                {
                    if (left)
                    {
                        wordArray[wordnum / 2 - n] = w.word;
                        left = false;
                    }
                    else
                    {
                        wordArray[wordnum / 2 + n] = w.word;
                        n++;
                        left = true;
                    }
                }
                


            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }

        }

        private void paintHead()
        {
            for (int i = 0; i < wordArray.Length; i++)
            {
                g.DrawString(
                    wordArray[i],
                    textFont1,
                    textBrush1,
                    (float)wordImage.Width-side,
                    (float)(side + i * wordsize)
                    );
                g.DrawString(
                    wordArray[i],
                    textFont1,
                    textBrush1,
                    (float)side,
                    (float)(side + i * wordsize)
                    );  
            }
        }
        private void paintHead2()
        {
            for (int i = 0; i < wordArray.Length; i++)
            {
                g.DrawString(
                    wordArray[i],
                    textFont1,
                    textBrush1,
                    (float)(side + i * wordsize),
                    (float)side - 10,
                    new StringFormat(StringFormatFlags.DirectionVertical)
                    );
                g.DrawString(
                    wordArray[i],
                    textFont1,
                    textBrush1,
                    (float)side,
                    (float)(side + i * wordsize),
                    new StringFormat(StringFormatFlags.DirectionRightToLeft)
                    );
            }
        }

        /// <summary>
        /// 打印单个字的图
        /// </summary>
        private void work1()
        {
            newBegin = true;
            isFirst = true;
            for (int i = 0; i < fileinfo.Length; i++)
            {
                if (getWordNum(fileinfo[i]) < 0)
                {
                    isFirst = true;
                    continue;
                }
                int newn = getWordNum(fileinfo[i]);
                if (newBegin)
                {
                    //句子首个字，不打印
                    newBegin = false;
                    isFirst = false;
                    lastn = newn;
                }
                else
                {
                    //后续字
                    paintNewLine(newn);
                    lastn = newn;
                    isFirst = isFirst ? false : true;
                }
                printInfo(i + "/" + fileinfo.Length);
            }
            printInfo(fileinfo.Length + "/" + fileinfo.Length+" 完成");
        }

        /// <summary>
        /// 打印词汇的图
        /// </summary>
        private void work2()
        {
            newBegin = true;
            isFirst = true;
            for (int i = 0; i < cutWordList.Length; i++)
            {
                if (getWordNum(cutWordList[i]) < 0)
                {
                    isFirst = true;
                    continue;
                }
                int newn = getWordNum(cutWordList[i]);
                if (newBegin)
                {
                    //句子首个字，不打印
                    newBegin = false;
                    isFirst = false;
                    lastn = newn;
                }
                else
                {
                    //后续字
                    paintNewLine(newn);
                    lastn = newn;
                    isFirst = isFirst ? false : true;
                }
                printInfo(i + "/" + cutWordList.Length);
            }
            printInfo(cutWordList.Length + "/" + cutWordList.Length + " 完成");
        }

        /// <summary>
        /// 打印词汇图2
        /// </summary>
        private void work3()
        {
            newBegin = true;
            isFirst = true;
            for (int i = 0; i < cutWordList.Length; i++)
            {
                int x = 0;
                int y = 0;
                if (cutWordList[i].Word.Length <= 1)
                {
                    isFirst = true;
                    continue;
                }
                if (getWordNum(cutWordList[i].Word[0]) < 0)
                {
                    isFirst = true;
                    continue;
                }
                x = getWordNum(cutWordList[i].Word[0]);
                if (getWordNum(cutWordList[i].Word[1]) < 0)
                {
                    isFirst = true;
                    continue;
                }
                y = getWordNum(cutWordList[i].Word[1]);
                if (newBegin)
                {
                    //句子首个字，不打印
                    newBegin = false;
                    isFirst = false;
                    lastp = new Point(x, y);
                }
                else
                {
                    //后续字
                    paintNewLine2(new Point(x, y));
                    lastp = new Point(x, y);
                    isFirst = isFirst ? false : true;
                }
                printInfo(i + "/" + cutWordList.Length);
            }
            printInfo(cutWordList.Length + "/" + cutWordList.Length + " 完成");

        }


        private void init()
        {
            try
            {
                wordnum = 0;

                word = new List<Word>();

                switch (type)
                {
                    case Type.Chars:
                        //逐字存储
                        for (int i = 0; i < fileinfo.Length; i++)
                        {
                            if (dropchar.IndexOf(fileinfo[i]) >= 0) continue;

                            bool get = false;
                            foreach (var w in word)
                            {
                                if (w.word == fileinfo[i].ToString())
                                {
                                    get = true;
                                    w.num++;
                                    break;
                                }
                            }
                            if (!get)
                            {
                                word.Add(new Word(fileinfo[i].ToString(), 1));
                            }
                        }
                        break;
                    case Type.Words:
                        //逐词存储
                        //分词
                        PosSegmenter segmenter = new PosSegmenter();
                        cutWordList = segmenter.Cut(fileinfo).ToArray();

                        for (int i = 0; i < cutWordList.Length; i++)
                        {
                            if (dropchar.IndexOf(cutWordList[i].Word) >= 0) continue;

                            bool get = false;
                            foreach (var w in word)
                            {
                                if (w.word == cutWordList[i].Word)
                                {
                                    get = true;
                                    w.num++;
                                    break;
                                }
                            }
                            if (!get)
                            {
                                word.Add(new Word(cutWordList[i].Word, 1));
                            }
                        }

                        break;
                    case Type.WordImage:
                        //分词
                        segmenter = new PosSegmenter();
                        cutWordList = segmenter.Cut(fileinfo).ToArray();
                        word2 = new List<Word>();
                        //逐词存储
                        for (int i = 0; i < cutWordList.Length; i++)
                        {
                            if (dropchar.IndexOf(cutWordList[i].Word) >= 0) continue;

                            bool get = false;
                            foreach (var w in word2)
                            {
                                if (w.word == cutWordList[i].Word)
                                {
                                    get = true;
                                    w.num++;
                                    break;
                                }
                            }
                            if (!get)
                            {
                                word2.Add(new Word(cutWordList[i].Word, 1));
                            }
                        }
                        word2.Sort();
                        Word[] tmpWord = (from a in word2 select a).Take(50).ToArray();
                        //逐字存储
                        for (int i = 0; i < tmpWord.Length; i++)
                        {
                            for (int j = 0; j < tmpWord[i].word.Length; j++)
                            {
                                bool get = false;
                                foreach (var w in word)
                                {
                                    if ((tmpWord[i].word)[j].ToString() == w.word)
                                    {
                                        get = true;
                                        w.num++;
                                        break;
                                    }
                                }
                                if (!get)
                                {
                                    word.Add(new Word((tmpWord[i].word)[j].ToString(), 1));
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }


                wordnum = word.Count();
                buildWordArray();
                wordImage = new Bitmap(wordnum * wordsize + 2 * side, wordnum * wordsize + 2 * side);
                this.g = Graphics.FromImage(wordImage);
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, wordImage.Width, wordImage.Height));
               


                switch (type)
                {
                    case Type.Chars:
                        paintHead();
                        new Thread(work1).Start();
                        break;
                    case Type.Words:
                        paintHead();
                        new Thread(work2).Start();
                        break;
                    case Type.WordImage:
                        paintHead2();
                        new Thread(work3).Start();
                        break;
                }
            }
            catch
            {
                printInfo("初始化出错。");
            }
            
            
        }
        /// <summary>
        /// 返回流的编码格式
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static Encoding getEncoding(string streamName)
        {
            Encoding encoding = Encoding.Default;
            using (Stream stream = new FileStream(streamName, FileMode.Open))
            {
                MemoryStream msTemp = new MemoryStream();
                int len = 0;
                byte[] buff = new byte[512];
                while ((len = stream.Read(buff, 0, 512)) > 0)
                {
                    msTemp.Write(buff, 0, len);
                }
                if (msTemp.Length > 0)
                {
                    msTemp.Seek(0, SeekOrigin.Begin);
                    byte[] PageBytes = new byte[msTemp.Length];
                    msTemp.Read(PageBytes, 0, PageBytes.Length);
                    msTemp.Seek(0, SeekOrigin.Begin);
                    int DetLen = 0;
                    UniversalDetector Det = new UniversalDetector(null);
                    byte[] DetectBuff = new byte[4096];
                    while ((DetLen = msTemp.Read(DetectBuff, 0, DetectBuff.Length)) > 0 && !Det.IsDone())
                    {
                        Det.HandleData(DetectBuff, 0, DetectBuff.Length);
                    }
                    Det.DataEnd();
                    if (Det.GetDetectedCharset() != null)
                    {
                        encoding = Encoding.GetEncoding(Det.GetDetectedCharset());
                    }
                }
                msTemp.Close();
                msTemp.Dispose();
                return encoding;
            }
        }

        private void workReadFile()
        {
            printInfo("文件读取中");
            string filename = openFileDialog1.FileName;
            Encoding encoding = Encoding.GetEncoding(getEncoding(filename).BodyName);
            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                StreamReader fr = new StreamReader(file, encoding);
                this.fileinfo = fr.ReadToEnd();
                fr.Dispose();
            }
            printInfo("文件读取完毕");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            new Thread(workReadFile).Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            type = Type.Chars;
            init();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            type = Type.Words;
            init();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            type = Type.WordImage;
            init();
        }
    }
}
