using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {

            foreach (var enumerableAllRegion in  RegionEndpoint.EnumerableAllRegions)
            {
                System.Console.Out.WriteLine(enumerableAllRegion.DisplayName+" "+enumerableAllRegion.SystemName);
            }

            System.Net.WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();

            AmazonS3Config config = new AmazonS3Config();
            config.ProxyHost = "localhost";
            config.ProxyPort = 3128;
            //config.ProxyCredentials = new 
            //config.RegionEndpoint =
            //config.UseHttp = true;

            var client = AWSClientFactory.CreateAmazonS3Client("", "", config);

            //client.
            var response = client.ListBuckets();
            foreach (var bucket in response.Buckets)
            {
                System.Console.Out.WriteLine(bucket.BucketName);
            }

            System.Console.Out.WriteLine("--end--");
            System.Console.ReadLine();
        }
    }
}
