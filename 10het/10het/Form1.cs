﻿using _10het.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _10het
{
    public partial class Form1 : Form
    {
        Random rng = new Random(1234);

        List<Person> Population = null;
        List<BirthProbability> BirthProbabilities = null;
        List<DeathProbability> DeathProbabilities = null;
        public Form1()
        {
            InitializeComponent();
            BirthProbabilities = GetBirthProbabilites(@"C:\Temp\születés.csv");
            DeathProbabilities = GetDeathProbabilites(@"C:\Temp\halál.csv");
        }

        private void StartSimulation(int year, string csvPath)
        {
            Population = GetPopulation(csvPath);

            for (int year = 2005; year <= EndYear; year++)
            {

                for (int i = 0; i < Population.Count; i++)
                {
                    SimStep(year, Population[i]);
                }

                int nbrOfMales = (from x in Population
                                  where x.Gender == Gender.Male && x.IsAlive
                                  select x).Count();
                int nbrOfFemales = (from x in Population
                                    where x.Gender == Gender.Female && x.IsAlive
                                    select x).Count();
                textBox1.Text += string.Format(
                    "Szimulációs év: {0}\n\tFiúk: {1}\n\tLányok: {2}\n\n", year, nbrOfMales, nbrOfFemales
                    );
            }
        }

        public List<Person> GetPopulation(string csvPath)
        {
            List<Person> population = new List<Person>();

            using (StreamReader sr = new StreamReader(csvPath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    var p = new Person();
                    p.BirthYear = int.Parse(line[0]);
                    p.Gender = (Gender)Enum.Parse(typeof(Gender), line[1]);
                    p.NbrOfChildren = int.Parse(line[2]);
                    population.Add(p);
                }
            }

            return population;
        }

        public List<BirthProbability> GetBirthProbabilites(string csvpath)
        {
            List<BirthProbability> birthprobabilities = new List<BirthProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    birthprobabilities.Add(new BirthProbability()
                    {
                        Age = int.Parse(line[0]),
                        P = double.Parse(line[2]),
                        NbrOfChildren = int.Parse(line[1])
                    });
                }
            }

            return birthprobabilities;
        }

        public List<DeathProbability> GetDeathProbabilites(string csvpath)
        {
            List<DeathProbability> deathprobabilities = new List<DeathProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    deathprobabilities.Add(new DeathProbability()
                    {
                        Age = int.Parse(line[1]),
                        P = double.Parse(line[2]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[0])
                    });
                }
            }

            return deathprobabilities;
        }

        private void SimStep(int year, Person person)
        {
            //Ha halott akkor kihagyjuk, ugrunk a ciklus következő lépésére
            if (!person.IsAlive) return;

            // Letároljuk az életkort, hogy ne kelljen mindenhol újraszámolni
            byte age = (byte)(year - person.BirthYear);

            // Halál kezelése
            // Halálozási valószínűség kikeresése
            double pDeath = (from x in DeathProbabilities
                             where x.Gender == person.Gender && x.Age == age
                             select x.P).FirstOrDefault();
            // Meghal a személy?
            if (rng.NextDouble() <= pDeath)
                person.IsAlive = false;

            //Születés kezelése - csak az élő nők szülnek
            if (person.IsAlive && person.Gender == Gender.Female)
            {
                //Szülési valószínűség kikeresése
                double pBirth = (from x in BirthProbabilities
                                 where x.Age == age
                                 select x.P).FirstOrDefault();
                //Születik gyermek?
                if (rng.NextDouble() <= pBirth)
                {
                    Person újszülött = new Person();
                    újszülött.BirthYear = year;
                    újszülött.NbrOfChildren = 0;
                    újszülött.Gender = (Gender)(rng.Next(1, 3));
                    Population.Add(újszülött);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) return;
            textBox1 = ofd.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartSimulation((int)numericUpDown1.Value, textBox1.Text);

        }
    }
}
