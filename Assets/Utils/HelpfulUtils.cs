using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Enums 
    {
        public enum Collider2DType
        {
            BoxCollider2D,
            CircleCollider2D,
            CapsuleCollider2D,
            PolygonCollider2D,
        }
        
        
        public enum Collider3DType
        {
            BoxCollider,
            SphereCollider,
            CapsuleCollider,
            MeshCollider,
            WheelCollider,
            TerrainCollider
        }
    }
    public class HelpfulUtils
    {
        
        public static string FormatTime(float timeInSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", 
                timeSpan.Hours, 
                timeSpan.Minutes, 
                timeSpan.Seconds);
        }
        
        public static string FormatTime(float timeInSeconds, bool includeMilliseconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
            if (includeMilliseconds)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", 
                    timeSpan.Hours, 
                    timeSpan.Minutes, 
                    timeSpan.Seconds, 
                    timeSpan.Milliseconds);
            }
            return string.Format("{0:D2}:{1:D2}:{2:D2}", 
                timeSpan.Hours, 
                timeSpan.Minutes, 
                timeSpan.Seconds);
        }
        
        
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