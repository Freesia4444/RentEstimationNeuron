using System;
using System.Collections.Generic;
using System.Linq;

namespace RentEstimationNeuron
{
    public class Neuron
    {
        private double[] agırlıklar;
        private double bias;
        private Random random;

        public Neuron(int inputCount, int? seed = null)
        {    // burada ternanry yapısı kullandım seed var mı kontrol ettim sonra varsa var olan degerle yoksa boş bir randomla olusturdum.
            random = seed.HasValue ? new Random(seed.Value) : new Random();
            agırlıklar = new double[inputCount];

           
            for (int i = 0; i < inputCount; i++)
            {
                agırlıklar[i] = random.NextDouble() * 2 - 1; 
            }
            bias = random.NextDouble() * 2 - 1;
            // burada bias kavramını kullandım cunku araştırmama gore bias kullanmadan kodu yazmak matemaiksel acıdan daha ugraştıcı
        }

        // dokumanda gecen y = 1/(1+e^(-v)) bu formulu uyguladım.
        private double Aktivasyon(double v)
        {
            return 1.0 / (1.0 + Math.Exp(-v));
        }

        // dokumanda anlatılan toplama işlemine gore yazdım
        public double Hesapla(double[] inputs)
        {
            double v = bias;
            for (int i = 0; i < inputs.Length; i++)
            {
                v += inputs[i] * agırlıklar[i];
            }
            return Aktivasyon(v);
        }

        
        public void Ogrenme(double[] inputs, double hedef, double lambda)
        {
            double output = Hesapla(inputs);
            double error = hedef - output;

            
            for (int i = 0; i < agırlıklar.Length; i++)
            {
                agırlıklar[i] += lambda * error * inputs[i];
            }
            bias += lambda * error * 1.0; 
        }

        public void agırliklarigösterme()
        {
            Console.WriteLine("ağırlıklari");
            for (int i = 0; i < agırlıklar.Length; i++)
            {
                Console.WriteLine($"w{i + 1}: {agırlıklar[i]:F6}");
            }
            Console.WriteLine($"bias ı  {bias:F6}");
        }
    }

   
    public class VeriOrnek
    {
        public double[] Inputs { get; set; }
        public double Hedef { get; set; }
        public int No { get; set; }

        public VeriOrnek(int no, double rooms, double distance, double age, double rent)
        {
            No = no;
            Inputs = new double[] { rooms, distance, age };
            Hedef = rent;
        }

        public VeriOrnek(int no, double rooms, double distance, double rent)
        {
            No = no;
            Inputs = new double[] { rooms, distance };
            Hedef = rent;
        }
    }

    class Program
    {
        
        static double MSEhesapla(List<VeriOrnek> data, Neuron neuron)
        {
            double kareHataToplam = 0;
            foreach (var ornek in data)
            {
                double tahmin = neuron.Hesapla(ornek.Inputs);
                double error = ornek.Hedef - tahmin;
                kareHataToplam += error * error;
            }
            return kareHataToplam / data.Count;
        }

       
        static List<VeriOrnek> NormalizeData(List<VeriOrnek> originalData, bool yasDahil = true)
        {
            List<VeriOrnek> normalized = new List<VeriOrnek>();

            foreach (var ornek in originalData)
            {
                if (yasDahil)
                {
                    normalized.Add(new VeriOrnek(
                        ornek.No,
                        ornek.Inputs[0] / 5.0,      
                        ornek.Inputs[1] / 20.0,     
                        ornek.Inputs[2] / 30.0,     
                        ornek.Hedef / 10000.0      
                    ));
                }
                else
                {
                    normalized.Add(new VeriOrnek(
                        ornek.No,
                        ornek.Inputs[0] / 5.0,      
                        ornek.Inputs[1] / 20.0,     
                        ornek.Hedef / 10000.0      
                    ));
                }
            }

            return normalized;
        }

        
        static void Sonuclarıgosterme(List<VeriOrnek> data, Neuron neuron, string baslik)
        {
            Console.WriteLine($"\n{baslik}");
            Console.WriteLine($"{"nosu",-5}{"girdi degerleri",-35}{"hedefi",-12}{"tahmini",-12}{"hatasıı",-10}");
            

            foreach (var ornek in data)
            {
                double tahmin = neuron.Hesapla(ornek.Inputs);
                double error = ornek.Hedef - tahmin;
                string inputs = $"[{string.Join(", ", ornek.Inputs.Select(x => x.ToString("F4")))}]";
                Console.WriteLine($"{ornek.No,-5}{inputs,-35}{ornek.Hedef,-12:F4}{tahmin,-12:F4}{error,-10:F4}"); // ondalık kıısmlarda gozuksun diye F4 yontemini kullandım
            }
        }

        
        static void TrainNeuron(Neuron neuron, List<VeriOrnek> trainingData, int epochs, double lambda)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                foreach (var ornek in trainingData)
                {
                    neuron.Ogrenme(ornek.Inputs, ornek.Hedef, lambda);
                }
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            
            List<VeriOrnek> trainingData = new List<VeriOrnek>
            {
                new VeriOrnek(1, 3, 4.8, 8, 7380),
                new VeriOrnek(2, 1, 15.2, 25, 543),
                new VeriOrnek(3, 4, 2, 5, 9221),
                new VeriOrnek(4, 2, 8.5, 12, 4305),
                new VeriOrnek(5, 5, 1.2, 2, 10727),
                new VeriOrnek(6, 3, 10, 15, 5553),
                new VeriOrnek(7, 2, 3.6, 6, 6500),
                new VeriOrnek(8, 4, 18, 20, 5109),
                new VeriOrnek(9, 1, 7.4, 10, 3058),
                new VeriOrnek(10, 5, 12.2, 30, 6716),
                new VeriOrnek(11, 2, 1, 5, 6778),
                new VeriOrnek(12, 3, 6.5, 7, 7345),
                new VeriOrnek(13, 4, 4, 9, 7898),
                new VeriOrnek(14, 1, 19.5, 28, 1270),
                new VeriOrnek(15, 5, 8.8, 4, 9692)
            };

            
            List<VeriOrnek> testData = new List<VeriOrnek>
            {
                new VeriOrnek(16, 3, 13.5, 20, 4948),
                new VeriOrnek(17, 2, 17, 15, 3227),
                new VeriOrnek(18, 4, 0.8, 3, 9789),
                new VeriOrnek(19, 5, 5, 6, 10123),
                new VeriOrnek(20, 1, 2.5, 2, 4741)
            };

            var normalizedTraining = NormalizeData(trainingData);
            var normalizedTest = NormalizeData(testData);

            Console.WriteLine("kira tahminii");

           
           
            Neuron neuron1 = new Neuron(3, seed: 42); 
            TrainNeuron(neuron1, normalizedTraining, 25, 0.05);

            neuron1.agırliklarigösterme();

            Sonuclarıgosterme(normalizedTraining, neuron1, "25 epok için egitim veri sonuçları");
            double trainMSE_25 = MSEhesapla(normalizedTraining, neuron1);
            Console.WriteLine($"\n Eğitim Verisi MSE (25 Epok Sonunda): {trainMSE_25:F6}");

            Sonuclarıgosterme(normalizedTest, neuron1, "25 epok için test verisi sonucları");
            double testMSE_25_005 = MSEhesapla(normalizedTest, neuron1);
            Console.WriteLine($"\n test verisi MSE yine 25 epok için {testMSE_25_005:F6}");

         
           

            double[,] testMSEResults = new double[2, 3]; 
            int[] epochs = { 25, 100 };
            double[] lambdadegerleri = { 0.01, 0.05, 0.1 };

            for (int e = 0; e < epochs.Length; e++)
            {
                for (int l = 0; l < lambdadegerleri.Length; l++)
                {
                    Neuron tempNeuron = new Neuron(3, seed: 42 + e * 3 + l); 
                    TrainNeuron(tempNeuron, normalizedTraining, epochs[e], lambdadegerleri[l]);
                    testMSEResults[e, l] = MSEhesapla(normalizedTest, tempNeuron);

                    Console.WriteLine($"Epok: {epochs[e],3}, λ: {lambdadegerleri[l]:F2}  Test MSE: {testMSEResults[e, l]:F6}");
                }
            }

           
            Console.WriteLine("\n\n 100 EPOK, λ=0.05 için test sonuçları ");
            Neuron neuron100 = new Neuron(3, seed: 42);
            TrainNeuron(neuron100, normalizedTraining, 100, 0.05);
            Sonuclarıgosterme(normalizedTest, neuron100, "TEST VERİSİ SONUÇLARI (100 Epok, λ=0.05)");
            double testMSE_100 = MSEhesapla(normalizedTest, neuron100);
            Console.WriteLine($"\n Test Verisi MSE (100 Epok): {testMSE_100:F6}");

           
            Console.WriteLine("mse degerlerii");
            Console.WriteLine($"{"Epok",-8}  {"λ = 0.01",-15}  {"λ = 0.05",-15}  {"λ = 0.1",-15}");
            for (int e = 0; e < epochs.Length; e++)
            {
                Console.WriteLine($" {epochs[e],-8}  {testMSEResults[e, 0],-15:F6}  {testMSEResults[e, 1],-15:F6}  {testMSEResults[e, 2],-15:F6} ");
            }
          
            
           

            List<VeriOrnek> trainingData2Input = new List<VeriOrnek>();
            foreach (var ornek in trainingData)
            {
                trainingData2Input.Add(new VeriOrnek(ornek.No, ornek.Inputs[0], ornek.Inputs[1], ornek.Hedef));
            }

            List<VeriOrnek> testData2Input = new List<VeriOrnek>();
            foreach (var ornek in testData)
            {
                testData2Input.Add(new VeriOrnek(ornek.No, ornek.Inputs[0], ornek.Inputs[1], ornek.Hedef));
            }

            var normalizedTraining2 = NormalizeData(trainingData2Input, false);
            var normalizedTest2 = NormalizeData(testData2Input, false);

            double[,] testMSEResults2Input = new double[2, 3];

            for (int e = 0; e < epochs.Length; e++)
            {
                for (int l = 0; l < lambdadegerleri.Length; l++)
                {
                    Neuron tempNeuron = new Neuron(2, seed: 100 + e * 3 + l); 
                    TrainNeuron(tempNeuron, normalizedTraining2, epochs[e], lambdadegerleri[l]);
                    testMSEResults2Input[e, l] = MSEhesapla(normalizedTest2, tempNeuron);

                    Console.WriteLine($"Epok: {epochs[e],3}, λ: {lambdadegerleri[l]:F2} Test MSE: {testMSEResults2Input[e, l]:F6}");
                }
            }


            Console.WriteLine("mse 2 girdi yaşsız");
            Console.WriteLine($"{"Epok",-8}  {"λ = 0.01",-15}  {"λ = 0.05",-15}  {"λ = 0.1",-15}");  
            for (int e = 0; e < epochs.Length; e++)
            {
                Console.WriteLine($" {epochs[e],-8}  {testMSEResults2Input[e, 0],-15:F6}  {testMSEResults2Input[e, 1],-15:F6}  {testMSEResults2Input[e, 2],-15:F6} ");
            }
            
            
           
        }
    }
}