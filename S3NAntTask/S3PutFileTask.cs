﻿using System;
using System.IO;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace S3NAntTask 
{
    [TaskName("amazon-s3-putFile")]
    public class S3PutFileTask : S3CoreFileTask
    {

        /// <summary>Execute the NAnt task</summary>
        protected override void ExecuteTask() 
        {
            // Ensure the configured bucket exists
            if (!BucketExists())
            {
                Project.Log(Level.Error, "[ERROR] S3 Bucket '{0}' not found!", BucketName);
                return;
            }

            // Ensure the specified file exists
            if (!File.Exists(FilePath)) 
            {
                Project.Log(Level.Error, "[ERROR] Local file '{0}' doesn't exist!", FilePath);
                return;
            }

            // Ensure the overwrite is false and the file doesn't already exist in the specified bucket
            if (!Overwrite && FileExists())
                return;

            // Send the file to S3
            using (Client) 
            {
                try 
                {
                    Project.Log(Level.Info, "Uploading file: {0}", FileName);
                    PutObjectRequest request = new PutObjectRequest 
                    {
                        Key = FileName,
                        BucketName = BucketName,
                        FilePath = FilePath,
                        Timeout = timeout
                    };

                    var response = Client.PutObject(request);

                }
                catch (AmazonS3Exception ex) 
                {
                    ShowError(ex);
                }
            }
            if (!FileExists())
                Project.Log(Level.Error, "File: {0}, failed to upload!", FileName);
            else
                Project.Log(Level.Info, "Successfully sent file to Amazon S3: {0}", FileName);
        }

    }
}