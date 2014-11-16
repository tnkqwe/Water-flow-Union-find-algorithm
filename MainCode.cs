using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WaterFlowUnionFindTest
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Cell
        {
            public bool opened;//отворено ли е
            public bool full;//пълна или празна
            public int ID;//номерът на клетката
            public int treeSize;
            public Cell root;//кой елемент стои с едно ниво надолу
            //методи==================================================================
            public Cell(int ID)
            {
                full = false;
                opened = false;
                this.ID = ID;
                treeSize = 1;
            }
            public Cell(bool isFull, int ID)//за горната (от където идва водата) и долната клетка (на където отива водата)
            {
                full = isFull;
                opened = true;
                this.ID = ID;
                treeSize = 1;
            }
            public void reset()
            {
                full = false;
                opened = false;
                root = null;
                treeSize = 1;
            }
            public Cell chainRoot()//търсене кой елемент е в основата на "дървото"
            {
                Cell tempCell = this;
                while (tempCell.root != null)//основата няма корен или root == null
                {
                    tempCell = tempCell.root;
                }
                //Рекурсия, мамка му!
                return tempCell;
            }
            /*
            public int chainLength()
            {
                Cell tempCell = this;
                int length = 1;
                while (tempCell.root != null)//основата няма корен или root == null
                {
                    tempCell = tempCell.root;
                    length++;
                }
                return length;
            }
            */
        }
        //методи за действия с клетки==============================================
        public bool connected(Cell cellA, Cell cellB)
        {
            if (cellA.chainRoot().Equals(cellB.chainRoot()))
                return true;
            else
                return false;
        }//свързани ли за двете клетки- имат ли еднаква основа на "дърветата" си
        public void connectCells(Cell cellA, Cell cellB)
        {
            if (!connected(cellA, cellB))
            {
                if (cellA.chainRoot().treeSize <= cellB.chainRoot().treeSize)
                {
                    cellB.chainRoot().treeSize += cellA.chainRoot().treeSize;
                    cellA.chainRoot().root = cellB.chainRoot();
                }
                else
                {
                    cellA.chainRoot().treeSize += cellB.chainRoot().treeSize;
                    cellB.chainRoot().root = cellA.chainRoot();
                }
                //cellA.chainRoot().root = cellB.chainRoot(); //без оптимизация
            }
        }
        //декларации===============================================================
        List<Cell> WaterCell;
        List<Cell> arrCell;//референции към клетките, сортирани са по това дали са отворени или не (за ускоряване на случайното отваряне на клетки)
        int[] sequence;
        Cell start;//клетката от която "идва водата"
        //Cell end;//когато тази клетка се свърже с началната (клетка start), всичко визуално свързано с end но не и със start се "напълва"
        List<Button> CellButton;
        int openedCells;//колко са отворените клетки
        int closedCells;//колко са затворените (за ускоряване на случайното отваряне на клетки)
        int cellNumbX;//колони
        int cellNumbY;//редове
        int maxCells;//колко клетки трябва да се отворят
        int totalCells;//всички клетки
        bool debug;//ако искам да дебъгвам
        //универсални методи=======================================================
        private void openCell(int cellID, bool paint)
        {
            if ((numericUpDown3.Value > 0 && openedCells >= numericUpDown3.Value) || (numericUpDown3.Value == 0 && openedCells >= cellNumbX * cellNumbY))
            {
                timer1.Enabled = false;
                button3.Text = "start";
                return;
            }
            //==============
            //код 0
            //==============
            WaterCell[cellID].opened = true;
            //нагоре
            if ((WaterCell[cellID].ID - cellNumbX) >= 0)
            {
                if (WaterCell[WaterCell[cellID].ID - cellNumbX].opened)
                {
                    connectCells(WaterCell[cellID], WaterCell[WaterCell[cellID].ID - cellNumbX]);
                    if (debug)
                        textBox1.AppendText("u");
                }
            }
            else
            {
                connectCells(WaterCell[cellID], start);
            }
            //надолу
            if ((WaterCell[cellID].ID + cellNumbX) < cellNumbX * cellNumbY)
            {
                if (WaterCell[WaterCell[cellID].ID + cellNumbX].opened)
                {
                    connectCells(WaterCell[cellID], WaterCell[WaterCell[cellID].ID + cellNumbX]);
                    if (debug)
                        textBox1.AppendText("d");
                }
            }
            /*
            else
            {
                connectCells(WaterCell[cellID], end);
            }
            */
            //надясно
            if ((WaterCell[cellID].ID + 1) % cellNumbX != 0)
            {
                if (WaterCell[WaterCell[cellID].ID + 1].opened)
                {
                    connectCells(WaterCell[cellID], WaterCell[WaterCell[cellID].ID + 1]);
                    if (debug)
                        textBox1.AppendText("r");
                }
            }
            //наляво
            if (WaterCell[cellID].ID % cellNumbX != 0)
            {
                if (WaterCell[WaterCell[cellID].ID - 1].opened)
                {
                    connectCells(WaterCell[cellID], WaterCell[WaterCell[cellID].ID - 1]);
                    if (debug)
                        textBox1.AppendText("l");
                }
            }
            if (paint)
                paintCell(cellID);
            openedCells++;
            if (waterFlowing())
            {
                label1.Text = "opened!";
                //connectCells(start, end);
            }
        }
        private void cellButtonClick(object sender, EventArgs e)//отваряне на клетки
        {
            Button SBC = (Button)sender;//SBC- Selected Button Cell
            int SWC = Convert.ToInt32(SBC.Name);//SWC- Selected Water Cell
            if (WaterCell[SWC].opened == false)
            {
                openCell(SWC, true);
            }
        }
        private void paintCell(int cellID)
        {
            if (CellButton[cellID].Text.Equals("X"))
            {
                CellButton[cellID].Text = "";
                CellButton[cellID].BackColor = System.Drawing.Color.White;
            }
            for (int counter = 0; counter < cellNumbX * cellNumbY; counter++)
            {
                if (connected(WaterCell[counter], start))
                {
                    CellButton[counter].BackColor = System.Drawing.Color.LightBlue;
                    WaterCell[counter].full = true;
                }
            }
        }
        private void closeAllCells(bool closeButtons)
        {
            for (int counter = 0; counter < cellNumbX * cellNumbY; counter++)
            {
                if (WaterCell[counter].opened)
                {
                    WaterCell[counter].reset();
                }
                if (closeButtons)
                    closeButtonCell(counter);
                start.root = null;
                //end.root = null;
                label1.Text = "closed";
                timer1.Enabled = false;
                openedCells = 0;
                closedCells = cellNumbX * cellNumbY;
                label2.Text = "0 / " + (cellNumbX * cellNumbY).ToString();
                arrCell[counter] = WaterCell[counter];
                button3.Text = "start";
            }
        }
        private void closeButtonCell(int cellID)
        {
            if (CellButton[cellID].Text != "X")
            {
                CellButton[cellID].Text = "X";
                CellButton[cellID].BackColor = System.Drawing.Color.Gray;
            }
        }
        private bool waterFlowing()
        {
            for (int c = cellNumbX * (cellNumbY - 1);
                c < cellNumbX * cellNumbY; c++)
                if (connected(WaterCell[c], start))
                    return true;
            return false;
        }
        //бутони===================================================================
        private void createBtn_Click(object sender, EventArgs e)//инициализиране
        {
            debug = false;
            genBlock = false;
            timer2Block = true;
            cellNumbX = (int)numericUpDown1.Value;
            cellNumbY = (int)numericUpDown2.Value;
            label2.Text = "0 / " + (cellNumbX * cellNumbY).ToString();
            openedCells = 0;
            procesedCombs = 0;
            openCombs = 0;
            closedCells = cellNumbX * cellNumbY;
            numericUpDown3.Maximum = cellNumbX * cellNumbY;
            if (cellNumbX >= 2 && cellNumbY >= 2)
            {
                WaterCell = new List<Cell>();
                arrCell = new List<Cell>();
                CellButton = new List<Button>();
                totalCells = cellNumbX * cellNumbY;
                for (int counter = 0; counter < cellNumbX * cellNumbY; counter++)
                {
                    Cell waterCell = new Cell(counter);
                    WaterCell.Add(waterCell);
                    Cell waterCellRef = WaterCell[counter];
                    arrCell.Add(waterCellRef);
                }
                start = new Cell(true, -1);
                //end = new Cell(false, -1);
                int count = 0;
                for (int counterY = 0; counterY < cellNumbY; counterY++)
                {
                    for (int counterX = 0; counterX < cellNumbX; counterX++)
                    {
                        Button cellButton = new Button();
                        cellButton.FlatStyle = FlatStyle.Flat;
                        cellButton.Location = new System.Drawing.Point(counterX * 20, counterY * 20);
                        cellButton.Name = count.ToString();
                        cellButton.Size = new System.Drawing.Size(20, 20);
                        cellButton.Text = "X";
                        cellButton.UseVisualStyleBackColor = true;
                        cellButton.Click += new System.EventHandler(cellButtonClick);
                        cellButton.Enabled = true;
                        cellButton.BackColor = System.Drawing.Color.Gray;
                        CellButton.Add(cellButton);
                        panel1.Controls.Add(CellButton[count]);
                        count++;
                    }
                }
                button3.Enabled = true;
                button1.Enabled = false;
                button2.Enabled = true;
                button4.Enabled = true;
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
                numericUpDown3.Enabled = true;
            }
        }
        private void startBtn_Click(object sender, EventArgs e)
        {
            //Button locBtn = (Button)sender;
            if (!timer1.Enabled)
            {
                timer1.Enabled = true;
                button3.Text = "pause";
                button6.Enabled = false;
            }
            else
            {
                timer1.Enabled = false;
                button3.Text = "start";
                button6.Enabled = true;
            }
        }
        private void resetBtn_Click(object sender, EventArgs e)
        {
            for (int counter = 0; counter < cellNumbY * cellNumbX; counter++)
            {
                WaterCell.Remove(WaterCell[0]);
                arrCell.Remove(arrCell[0]);
                this.panel1.Controls.Remove(CellButton[0]);
                CellButton.Remove(CellButton[0]);
            }
            start.root = null;
            //end.root = null;
            sequence = null;
            button1.Enabled = true;
            button3.Enabled = false;
            button3.Text = "start";
            button2.Enabled = false;
            button4.Enabled = false;
            button6.Enabled = false;
            numericUpDown1.Enabled = true;
            numericUpDown2.Enabled = true;
            numericUpDown3.Enabled = false;
            label1.Text = "closed";
            label2.Text = "0 / 0";
        }
        private void debugBtn_Click(object sender, EventArgs e)
        {
            if (debug)
                debug = false;
            else
                debug = true;
        }
        private void closeCellsBtn_Click(object sender, EventArgs e)
        {
            closeAllCells(true);
        }
        //случайно отваряне========================================================
        private void randomOppening(object sender, EventArgs e)
        {
            Random rnd = new Random();
            if (debug)
                textBox1.AppendText("R");
            int rand = rnd.Next(0, closedCells);
            openCell(arrCell[rand].ID, true);
            label2.Text = openedCells.ToString() + " / " + (cellNumbX * cellNumbY).ToString();
            //сортиране на масива с референции==================
            if (closedCells > 0)
            {
                Cell tempCellA = arrCell[rand];
                Cell tempCellB = arrCell[closedCells - 1];
                arrCell[rand] = tempCellB;
                arrCell[closedCells - 1] = tempCellA;
                closedCells--;
                if (debug)
                    textBox1.AppendText("s");
            }
            //==================================================
        }//timer1
        //генератор на шансове=====================================================
        public class SimpleCell
        {
            public SimpleCell root;
            public int rootLength;
            public bool opened;
            public SimpleCell()
            {
                reset(); //двете правят едно и също...
            }
            public void reset()
            {
                root = null;
                rootLength = 1;
                opened = false;
            }
            public SimpleCell chainRoot()
            {
                SimpleCell tempCell = this;
                while (tempCell.root != null)
                    tempCell = tempCell.root;
                return tempCell;
            }
        }
        SimpleCell[] simpleCell;
        int procesedCombs;//всички възможни комбинации
        int openCombs;//при колко комбинации "водата тече"
        bool genBlock;//когато тестерът работи, генераторът е блокиран
        ManualResetEvent genBlocker;
        bool genPause;
        bool timer2Block;//когато генераторър работи, тестерът е блокиран
        bool abortTime;//спиране на нишката
        bool done;//готов ли е генраторът
        //bool genStop;//генераторът ли е блокиран при даване на пауза; false -> таймерът е блокиран
        Thread t;
        private bool connected(SimpleCell A, SimpleCell B)
        {
            if (A.chainRoot().Equals(B.chainRoot()))
                return true;
            else
                return false;
        }
        private void connect(SimpleCell A, SimpleCell B)
        {
            if (!connected(A, B))
            {
                if (A.chainRoot().rootLength <= B.chainRoot().rootLength)
                {
                    B.chainRoot().rootLength += A.chainRoot().rootLength;
                    A.chainRoot().root = B.chainRoot();
                }
                else
                {
                    A.chainRoot().rootLength += B.chainRoot().rootLength;
                    B.chainRoot().root = A.chainRoot();
                }
            }
        }
        private void openSimpleCell(int ID)
        {
            simpleCell[ID].opened = true;
            //нагоре
            if (ID - cellNumbX >= 0) //ако е най-горе
            {
                if (simpleCell[ID - cellNumbX].opened)
                    connect(simpleCell[ID], simpleCell[ID - cellNumbX]);
            }
            else
                connect(simpleCell[ID], simpleCell[cellNumbY * cellNumbX]);
            //надолу
            if (ID + cellNumbX < cellNumbY * cellNumbX) //ако е най-долу
            {
                if (simpleCell[ID + cellNumbX].opened)
                    connect(simpleCell[ID], simpleCell[ID + cellNumbX]);
            }
            //надясно
            if ((ID + 1) % cellNumbX != 0) //ако е в най-дясната колона
            {
                if (simpleCell[ID + 1].opened)
                    connect(simpleCell[ID], simpleCell[ID + 1]);
            }
            //наляво
            if (ID % cellNumbX != 0) //ако е в най-лявата колона
            {
                if (simpleCell[ID - 1].opened)
                    connect(simpleCell[ID], simpleCell[ID - 1]);
            }
        }
        private bool waterFlowingSC()
        {
            for (int c = cellNumbX * (cellNumbY - 1);
                c < cellNumbX * cellNumbY; c++)
                //дали поне една от клетките в най-долния ред е свързана за "извора"
                if (connected(simpleCell[c], simpleCell[cellNumbX * cellNumbY]))//клетка номер cellNumbX * cellNumbY е клетката от която идва вода
                    return true;
            return false;
        }
        private void closeAllSimpleCells()
        {
            for (int c = 0; c < cellNumbX * cellNumbY; c++)
                simpleCell[c].reset();
            simpleCell[cellNumbX * cellNumbY].opened = true;
        }
        private void calcChanceBtn_Click(object sender, EventArgs e)
        {
            simpleCell = new SimpleCell [cellNumbX * cellNumbY + 1];
            for (int c = 0; c <= cellNumbX * cellNumbY; c++)
            {
                simpleCell[c] = new SimpleCell();
            }
            simpleCell [cellNumbX * cellNumbY].opened = true;
            maxCells = (int)numericUpDown3.Value;
            sequence = new int[maxCells];
            openCombs = 0;
            procesedCombs = 0;
            abortTime = false;
            timer2Block = true;
            genBlock = false;
            genPause = false;
            genBlocker = new ManualResetEvent(true);
            timer2.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = true;
            button8.Enabled = true;
            //for (int c = 0; c < maxCells; c++)
            //{
            //    sequence[c] = c;
            //}
            t = new Thread(combGenerator);
            t.Start();
        }
        private void combTester(object sender, EventArgs e)
        {
            if (!genPause && !timer2Block)
            {
                //преди да направя SimpleCell за тестване на случаите, ползвах Cell.
                //След като се пробвах да преместя този код в нишката, трябваше да правя
                //копия на някой от методите, но не се получи и реших да напиша чисто нов код
                /*
                closeAllCells(false);
                for (int c = 0; c < sequence.Length; c++)
                {
                    openCell(sequence[c], false);
                }
                procesedCombs++;
                */
                //=======================================================================
                //новият код е тук за тестване
                /*
                closeAllSimpleCells();
                for (int c = 0; c < sequence.Length; c++)
                {
                    openSimpleCell(sequence[c]);
                }
                */
                //if (waterFlowingSC())
                //{
                    label1.Text = "opened!";
                    openCombs++;
                    closeAllCells(false);
                    for (int c = 0; c < totalCells; c++)
                    {
                        closeButtonCell(c);
                    }
                    for (int c = 0; c < maxCells; c++)
                    {
                        //paintCell(sequence[c]);
                        openCell(sequence[c], true);
                    }
                //}
                label3.Text = openCombs + " / " + procesedCombs;
                if (debug)
                {
                    textBox1.Text = "tested ";
                    for (int c = 0; c < sequence.Length; c++)
                    {
                        textBox1.AppendText(sequence[c] + " ");
                    }
                    textBox1.AppendText(timer2Block + " " + genBlock + " ");
                }
                //=================
                //код 1
                //=================
                //genBlock = false;
                timer2Block = true;
                genBlocker.Set();//освобождаване на генераторът
            }
            if (done)
                timer2.Enabled = false;
            if (t.IsAlive == false)
            {
                stopGen();
            }
        }//timer2 тества комбинациите
        private void combGenerator()
        {
            combGenerator(0, -1);
            done = true;
        }//успях да направя отделна нишка да работи... УСПЯХ!
        private void combGenerator(int currEl, int begVal)
        {
            sequence[currEl] = begVal;
            for (int c = begVal; c <= currEl + totalCells - maxCells; c++)
            {
                /*
                while (genBlock && !abortTime)
                {
                    if (abortTime)
                        return;
                }
                */
                if (abortTime)
                    return;
                if (currEl == maxCells - 1)
                {
                    if (sequence[currEl] + 1 < totalCells)
                    {
                        sequence[currEl]++;
                        closeAllSimpleCells();
                        for (int c0 = 0; c0 < sequence.Length; c0++)
                        {
                            openSimpleCell(sequence[c0]);
                        }
                        procesedCombs++;
                        if (waterFlowingSC())
                        {
                            genBlocker.Reset();//казва на genBlocker.WaitOne да чака за genBlocker.Set
                            genBlock = true;
                            timer2Block = false;
                            genBlocker.WaitOne();//блокиране на нишката
                        }
                    }
                }
                else
                {
                    sequence[currEl]++;
                    combGenerator(currEl + 1, sequence[currEl]);
                    begVal++;
                }
            }
        }//генератор за комбинации
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                while (t.IsAlive)
                {
                    abortTime = true;
                    genBlocker.Set();
                }
            }
            catch (NullReferenceException) { }
        }
        private void stopGenBtn_Click(object sender, EventArgs e)
        {
            stopGen();
            label3.Text = "0 / 0";
        }
        private void stopGen()
        {
            while (t.IsAlive)
            {
                abortTime = true;
                genBlocker.Set();
            }
            timer2.Enabled = false;
            done = false;
            closeAllCells(true);
            sequence = null;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = false;
            button8.Enabled = false;
        }
        private void pauseGenBtn_Click(object sender, EventArgs e)
        {
            if (!genPause)
            {
                genPause = true;
                button7.Text = "resume calc.";
            }
            else
            {
                genPause = false;
                button7.Text = "pause calc.";
            }
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            if ((int)numericUpDown3.Value == 0)
                button6.Enabled = false;
            else
                button6.Enabled = true;
        }
    }
}
