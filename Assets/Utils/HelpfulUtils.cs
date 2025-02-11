using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class HelpfulUtils
    {
        public static string GenerateGUID()
        {
            return Guid.NewGuid().ToString();
        }

        public static string FormatFloat(float value, int precision = 2)
        {
            return value.ToString("F" + precision);
        }

        public static float SumList(List<float> numbers)
        {
            float sum = 0f;
            foreach (float num in numbers)
                sum += num;
            return sum;
        }

        public static float MultiplyList(List<float> numbers)
        {
            float product = 1f;
            foreach (float num in numbers)
                product *= num;
            return product;
        }

        public static float FindMedian(List<float> numbers)
        {
            if (numbers == null || numbers.Count == 0)
                throw new ArgumentException("List is empty");
            numbers.Sort();
            int count = numbers.Count;
            return (count % 2 == 0) ? (numbers[count / 2 - 1] + numbers[count / 2]) / 2f : numbers[count / 2];
        }

        public static float FindMean(List<float> numbers)
        {
            if (numbers == null || numbers.Count == 0)
                throw new ArgumentException("List is empty");
            float sum = 0f;
            foreach (float num in numbers)
                sum += num;
            return sum / numbers.Count;
        }

        public static float FindStandardDeviation(List<float> numbers)
        {
            float mean = FindMean(numbers);
            float sumSq = 0f;
            foreach (float num in numbers)
                sumSq += (num - mean) * (num - mean);
            return Mathf.Sqrt(sumSq / numbers.Count);
        }

        public static string GenerateRandomPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            System.Text.StringBuilder res = new System.Text.StringBuilder();
            for (int i = 0; i < length; i++)
                res.Append(valid[UnityEngine.Random.Range(0, valid.Length)]);
            return res.ToString();
        }
    }
}