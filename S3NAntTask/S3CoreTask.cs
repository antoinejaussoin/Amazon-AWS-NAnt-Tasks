﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace S3NAntTask
{

    public abstract class S3CoreTask : Task
    {
        #region Task Attributes

        /// <summary>Region to create the new bucket in. Default to US standard</summary>
        public S3Region _region = S3Region.US;
        public int timeout = 3600000;

        [TaskAttribute("accesskey", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string AWSAccessKey { get; set; }

        [TaskAttribute("secretkey", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string AWSSecretKey { get; set; }

        [TaskAttribute("bucket", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string BucketName { get; set; }

        [TaskAttribute("proxyhost", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string ProxyHost { get; set; }

        [TaskAttribute("proxyport", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string ProxyPort { get; set; }

        [TaskAttribute("proxyusername", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string ProxyUsername { get; set; }

        [TaskAttribute("proxypassword", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string ProxyPassword { get; set; }

        [TaskAttribute("region", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string Region
        {
            get
            {
                return _region.ToString();
            }
            set
            {
                _region = (S3Region)Enum.Parse(typeof(S3Region), value);
                Project.Log(Level.Info, String.Format("Set Amazon region to: {0}", _region));
            }
        }

        #endregion

        /// <summary>Get an Amazon S3 client. Be sure to dispose of the client when done</summary>
        public IAmazonS3 Client
        {
            get
            {
                var config = new AmazonS3Config();

                config.RegionEndpoint = RegionEndpoint.EUWest1;

                if (!string.IsNullOrEmpty(ProxyHost))
                {
                    config.ProxyHost = ProxyHost;
                    config.ProxyPort = string.IsNullOrEmpty(ProxyPort) ? 8080 : int.Parse(ProxyPort);
                }

                if (!string.IsNullOrEmpty(ProxyUsername))
                {
                    config.ProxyCredentials = new NetworkCredential(ProxyUsername, ProxyPassword);
                }

                return Amazon.AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretKey, config);
            }
        }

        /// <summary>Determine if the specified bucket alredy exists</summary>
        /// <returns>True if the bucket exists</returns>
        public bool BucketExists(string bucketName)
        {
            using (Client)
            {
                try
                {
                    var response = Client.ListBuckets();
                    {
                        if (response.Buckets.Any(bucket => bucket.BucketName.Equals(bucketName)))
                        {
                            return true;
                        }

                    }
                }
                catch (AmazonS3Exception ex)
                {
                    ShowError(ex);
                }
            }
            return false;
        }

        /// <summary>Format and display an exception</summary>
        /// <param name="ex">Exception to display</param>
        public void ShowError(AmazonS3Exception ex)
        {
            if (ex.ErrorCode != null && (ex.ErrorCode.Equals("InvalidAccessKeyId") || ex.ErrorCode.Equals("InvalidSecurity")))
            {
                Project.Log(Level.Error, "Please check the provided AWS Credentials.");
            }
            else
            {
                Project.Log(Level.Error, "An Error, number {0}, occurred with the message '{1}'",
                    ex.ErrorCode, ex.Message);
            }
        }  
 
    }
}
