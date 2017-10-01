/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WNMF.Common.Definition;
using WNMF.Common.Foundation;
using WNMF.Common.MicroService;
using WNMF.Common.Protcols.File;

namespace WNMF.Test
{
    [TestClass]
    public class ProofOfConceptTests
    {
        private SimpleRamPublishingHistory _publishingHistory;
        private NetworkEndpointsManager _endpoints;
        private FileBasedNetworkMessageHandler _handler;
        private SimpleNetworkMessagePublisher _distro;
        private DirectoryInfo _dropDir;
        private DirectoryInfo _output;
        private DirectoryInfo _input;

        [TestInitialize]
        public void Initialize() {
            var rootdir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            rootdir.Create();
            _input = rootdir.CreateSubdirectory("INPUT");
            _dropDir = rootdir.CreateSubdirectory("STAGING");
            _output = rootdir.CreateSubdirectory("OUTPUT");
            _publishingHistory = new SimpleRamPublishingHistory();
            _endpoints = new NetworkEndpointsManager();
            _endpoints.TryAddEndPoint(
                new FileEndPoint(new Uri(_output.FullName)), out _);
            _handler = new FileBasedNetworkMessageHandler(null, _dropDir.FullName);
            _distro = new SimpleNetworkMessagePublisher("test",_endpoints, _handler, _publishingHistory);
        }

        [TestMethod]
        public void CanGetServices() {
            _distro.TryGetService<INetworkEndpointsManager>(out var graphService);
            Assert.AreSame(_endpoints,graphService.Data);

            _distro.TryGetService<INetworkMessagePublishingHistory>(out var historyService);
            Assert.AreSame(_publishingHistory, historyService.Data);

            _distro.TryGetService<INetworkMessageHandler>(out var handlerService);
            Assert.AreSame(_handler, handlerService.Data);
        }

        [TestMethod]
        public void DemoMicroService() {
            using (var micro = new MicroServiceBase(_distro)) {

                var startCount = _output.EnumerateFiles().Count()/2;
                var timer=new Stopwatch();
                timer.Start();
                micro.Publish(CreateStream());
                micro.Publish(CreateStream());
                micro.Publish(CreateStream());
                micro.Publish(CreateStream());
                micro.Publish(CreateStream());
                int currentCount;
                do {
                     currentCount = _output.EnumerateFiles().Count()/2;
                    Console.WriteLine("{0} = {1}", timer.Elapsed, currentCount);
                    if(timer.Elapsed.TotalSeconds>5)
                        Assert.Fail();
                    Thread.Sleep(10);
                } while ((currentCount-5)<startCount);
                timer.Stop();

            }
        }

        [TestMethod]
        public void CanMoveFIles() {
            StageTestFile();
            StageTestFile();
            StageTestFile();
            StageTestFile();
            StageTestFile();
            var successRatio = _distro.Pump(out var results);
            Console.WriteLine("DIR={0}", _output.Parent);
            Console.WriteLine("OUTDIR={0}", _output);

            Console.WriteLine("SR={0}", successRatio);
            ReportResults(results);

            Assert.AreEqual(1.0,successRatio);

            successRatio = _distro.Pump(out results);
            Console.WriteLine("DIR={0}", _output.Parent);
            Console.WriteLine("OUTDIR={0}", _output);

            Console.WriteLine("SR={0}", successRatio);
            ReportResults(results);

            Assert.AreEqual(double.NaN, successRatio);
        }

        private void StageTestFile() {
            var stream = CreateStream();
            var stagingSuccess = _distro.Stage(stream,
                out var stageRsults);

            ReportResults(stageRsults);
            Assert.IsTrue(stagingSuccess);
        }

        private SimpleThreadLocalStream CreateStream() {
            var inputFile = CreateTestInputFIle(out var originalDescription);
            var stream = new SimpleThreadLocalStream(
                (d) => File.OpenRead(inputFile),
                originalDescription
            );
            return stream;
        }

        private string CreateTestInputFIle(out NetworkMessageDescription originalDescription) {
            var msg = Guid.NewGuid().ToString();
            var inputFile = Path.Combine(_input.FullName,DateTime.Now.Ticks+"file.txt");
            File.WriteAllText(inputFile, msg);
            var inputFileInfo = new FileInfo(inputFile);
            originalDescription = new NetworkMessageDescription(
                inputFileInfo,
                $"File={inputFileInfo.Name};Src={Environment.MachineName};MimeType=text/plain");
            return inputFile;
        }

        private static void ReportResults(List<TryOperationResponseBase> results) {
            foreach (var result in results) {
                Console.WriteLine("\r\n{0}:", result.MessageType);
                Console.WriteLine("\tMessage Type={0}:", result.MessageType);
                Console.WriteLine("\tException={0}:", result.Exception);
            }
        }
    }
}
