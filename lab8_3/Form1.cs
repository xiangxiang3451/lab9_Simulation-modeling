using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.Distributions;

namespace lab8_3
{
    public partial class Form1 : Form
    {
        private double[] probabilities = new double[5]; // Array to store event probabilities
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            InitializeProbabilities();
        }

        private void InitializeProbabilities()
        {
            TextBox[] probabilityTextBoxes = { textBox1, textBox2, textBox3, textBox4, textBox5 };

            // Get probabilities from text boxes and initialize the array
            for (int i = 0; i < probabilities.Length; i++)
            {
                double probability;
                if (double.TryParse(probabilityTextBoxes[i].Text, out probability))
                {
                    probabilities[i] = probability;
                }
                else
                {
                    probabilities[i] = 0.2;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            InitializeProbabilities();
            if (!ValidateProbabilities())
            {
                MessageBox.Show("Please ensure the total probability of all events is 1!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox6.Text, out int trials) || trials <= 0)
            {
                MessageBox.Show("Please enter a valid number of trials!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int[] eventCounts = new int[probabilities.Length]; // Array to store the number of occurrences of each event

            // Generate random numbers and update event occurrence counts
            for (int i = 0; i < trials; i++)
            {
                double randomNumber = random.NextDouble();

                double cumulativeProbability = 0;
                for (int j = 0; j < probabilities.Length; j++)
                {
                    cumulativeProbability += probabilities[j];
                    if (randomNumber < cumulativeProbability)
                    {
                        eventCounts[j]++;
                        break;
                    }
                }
            }

            double[] meanAndVariance = CalculateMeanAndVariance(eventCounts, trials); // Calculate Average and variance
            double chiSquare = CalculateChiSquare(eventCounts, trials); // Calculate chi-square statistic
            double pValue = ChiSquared.InvCDF(eventCounts.Length - 1, 1 - 0.05); // Set significance level to 0.05 and calculate critical value

            UpdateChart(eventCounts, trials);

            // Display results
            MessageBox.Show($"Average: {meanAndVariance[0]}\nVariance: {meanAndVariance[1]}\n" +
                $"Relative Error of Average: {meanAndVariance[2]}\nRelative Error of Variance: {meanAndVariance[3]}\n" +
                $"Chi-Square Statistic: {chiSquare}\nCritical Value: {pValue}\n" +
                $"Chi-Square Test Result: {(chiSquare > pValue ? "chiSquare > pValue is true" : "chiSquare > pValue is false")}", "Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private double[] CalculateMeanAndVariance(int[] eventCounts, int trials)
        {
            double[] meanAndVariance = new double[4]; // [Average, Variance, Relative Error of Average, Relative Error of Variance]
            double sum = 0;
            double sumOfSquares = 0;

            // Calculate sum of Average and variance
            for (int i = 0; i < eventCounts.Length; i++)
            {
                sum += eventCounts[i] * probabilities[i];
                sumOfSquares += eventCounts[i] * probabilities[i] * probabilities[i];
            }

            double mean = sum / trials;
            double variance = sumOfSquares / trials - mean * mean;

            // Calculate relative errors
            double relativeErrorMean = Math.Sqrt(variance / trials) / mean;
            double relativeErrorVariance = Math.Sqrt(2.0 / trials) / Math.Sqrt(variance);

            meanAndVariance[0] = mean;
            meanAndVariance[1] = variance;
            meanAndVariance[2] = relativeErrorMean;
            meanAndVariance[3] = relativeErrorVariance;

            return meanAndVariance;
        }
        private double CalculateChiSquare(int[] eventCounts, int trials)
        {
            double chiSquare = 0;
            double[] expectedCounts = new double[eventCounts.Length];

            // Calculate expected counts
            for (int i = 0; i < expectedCounts.Length; i++)
            {
                expectedCounts[i] = probabilities[i] * trials;
            }

            // Calculate chi-square statistic
            for (int i = 0; i < eventCounts.Length; i++)
            {
                chiSquare += Math.Pow(eventCounts[i] - expectedCounts[i], 2) / expectedCounts[i];
            }

            return chiSquare;
        }

        private bool ValidateProbabilities()
        {
            double sum = probabilities.Sum();
            return sum >= 0.999 && sum <= 1.001;
        }

        private void UpdateChart(int[] eventCounts, int trials)
        {
            chart1.Series.Clear();
            chart1.Series.Add("Event Frequency");
            // Add data
            for (int i = 0; i < eventCounts.Length; i++)
            {
                double frequency = (double)eventCounts[i] / trials;
                chart1.Series["Event Frequency"].Points.AddXY($"Event {i + 1}", frequency);
            }
        }
    }
}
