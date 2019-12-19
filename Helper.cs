using System;
using System.Collections.Generic;

namespace QuantumPerceptron
{
    public class Helper
    {
        public static long[] ConvertToLongArray(string str)
        {
            string[] inputStr = str.Split(",");
            long[] intArray = new long[inputStr.Length];
            for (int i = 0; i < inputStr.Length; i++)
            {
                intArray[i] = Convert.ToInt32(inputStr[i]);
            }
            return intArray;
        }

        public static double DotProduct(long[] inputVector, long[] weightVector)
        {
            long prod = 0;

            for (int i = 0; i < inputVector.Length; i++)
            {
                prod += inputVector[i] * weightVector[i];
            }

            return prod;
        }

        /// <summary>
        /// Method to calculate Controlled Z indexes based on the input vector
        /// </summary>
        /// <param name="inputVector">Input Vector for Perceptron in -1/+1</param>
        /// <param name="preComputedOnesListDictionary">Input Weight for Perceptron in -1/+1</param>
        /// <returns></returns>
        public static List<List<long>> CalculateInputZsList(long[] inputVector, Dictionary<long, Dictionary<long, List<long>>> preComputedOnesListDictionary)
        {
            if (inputVector[0] == -1)
            {
                for (int i = 0; i < inputVector.Length; i++)
                {
                    inputVector[i] *= -1;
                }
            }

            return CalculateControlledZPositions(inputVector, preComputedOnesListDictionary);
        }

        /// <summary>
        /// Method to retrieve maps of bitset count to the numbers and their corresponding bit set position.
        /// Each entry will be map of number of ones set to numbers and the position at which bit is set.
        /// </summary>
        /// <param name="bitCount"></param>
        /// <returns></returns>
        /**
         * Map for n = 3
         * 1: [1:[0], 2: [1], 4: [2]]
         * 2: [3:[0,1], 5:[0,2], 6: [1,2]]
         * 3: [7:[0,1,2]]
         * */
        public static Dictionary<long, Dictionary<long, List<long>>> CreateBitSetListForNumbers(int bitCount)
        {
            // Maximum number for the QubitCount: 2^qubitCount
            long maxNum = (long)Math.Pow(2.0, bitCount * 1.0);
            long[] maxNumForAllBits = new long[bitCount];

            // Precomputing all the max number with i bits: 2^i
            for (int i = 0; i < bitCount; i++)
            {
                maxNumForAllBits[i] = (long)Math.Pow(2.0, i * 1.0);
            }

            Dictionary<long, Dictionary<long, List<long>>> bitSetCountToNumberAndBiSetPositionMap = new Dictionary<long, Dictionary<long, List<long>>>();

            // Finding the number of bits set and their position for all the numbers from 1 to maxNum
            for (int i = 1; i < maxNum; i++)
            {
                int bitSetCount = 0;
                Dictionary<long, List<long>> numberBitSetPositionMap = new Dictionary<long, List<long>>();
                List<long> onesTempList = new List<long>();

                for (int j = 0; j < bitCount; j++)
                {
                    // checking if jth bit is set for number i
                    long temp = i & (long)maxNumForAllBits[j];
                    if (temp != 0)
                    {
                        // adding the jth position value to list
                        onesTempList.Add(j);
                        // increasing the count of bit
                        bitSetCount++;
                    }
                }

                bitSetCountToNumberAndBiSetPositionMap.TryGetValue(bitSetCount, out numberBitSetPositionMap);
                if (numberBitSetPositionMap == null)
                {
                    numberBitSetPositionMap = new Dictionary<long, List<long>>();
                }

                // adding the bit set position for number to the map
                numberBitSetPositionMap.Add(i, onesTempList);
                // updating the map for the bitSetCount
                bitSetCountToNumberAndBiSetPositionMap[bitSetCount] = numberBitSetPositionMap;
            }

            return bitSetCountToNumberAndBiSetPositionMap;
        }

        /// <summary>
        /// Method to return the List of Combinations of Bits' position to apply Controlled Z
        /// </summary>
        /// <param name="inputVector"></param>
        /// <param name="bitSetCountAndPositionMap"></param>
        /// <returns></returns>
        /**
         * one example for n = 3
         * [[1,2], [2,3], [1,2,3]]
         * */
        public static List<List<long>> CalculateControlledZPositions(long[] inputVector, Dictionary<long, Dictionary<long, List<long>>> bitSetCountAndPositionMap)
        {
            long[] inProgressVector = new long[inputVector.Length];
            Array.Fill(inProgressVector, 1);
            List<List<long>> result = new List<List<long>>();

            foreach (KeyValuePair<long, Dictionary<long, List<long>>> entry in bitSetCountAndPositionMap)
            {
                foreach (KeyValuePair<long, List<long>> entry1 in entry.Value)
                {
                    if (inputVector[entry1.Key] != inProgressVector[entry1.Key])
                    {
                        List<long> bitSetPositionList = entry1.Value;
                        result.Add(bitSetPositionList);

                        // Flip all the number greater than equal to Decimal number(entry1.key)
                        for (long i = entry1.Key; i < inputVector.Length; i++)
                        {
                            int bitSetCount = 0;
                            for (int j = 0; j < bitSetPositionList.Count; j++)
                            {
                                long tempPow = 1 << (int)bitSetPositionList[j];

                                if ((i & tempPow) > 0)
                                {
                                    bitSetCount++;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (bitSetCount == bitSetPositionList.Count)
                            {
                                inProgressVector[i] *= -1;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Construct HyperGraph 
        /// </summary>
        /// <param name="listCPZ"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool[] ConstructHyperGraph(List<List<long>> listCPZ, int n)
        {
            bool[] output = new bool[(1 << n) - 1];

            for (int i = 0; i < ((1 << n) - 1); i++)
            {
                output[i] = false;
            }

            foreach (List<long> cpz in listCPZ)
            {
                int index = -1;

                foreach (int bitIndex in cpz)
                {
                    index += 1 << bitIndex;
                }

                output[index] = true;
            }

            return output;
        }
    }
}
