/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WNMF.Common.Definition;
using WNMF.Common.Foundation;
using WNMF.Common.Protcols.File;

namespace WNMF.Test
{
    [TestClass]
    public class ProofOfConceptTests
    {
        private SimpleRamHistory _history;
        private NetworkEndpointsManager _endpoints;
        private FileBasedNetworkMessageHandler _handler;
        private SimpleNetworkMessageDistributor _distro;
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
            _history = new SimpleRamHistory();
            _endpoints = new NetworkEndpointsManager();
            _endpoints.TryAddEndPoint(
                new FileEndPoint(new Uri(_output.FullName)), out _);
            _handler = new FileBasedNetworkMessageHandler(null, _dropDir.FullName);
            _distro = new SimpleNetworkMessageDistributor(_endpoints, _handler, _history);
        }

        [TestMethod]
        public void CanGetServices() {
            _distro.TryGetService<INetworkEndpointsManager>(out var graphService);
            Assert.AreSame(_endpoints,graphService.Data);

            _distro.TryGetService<INetworkMessageHistory>(out var historyService);
            Assert.AreSame(_history, historyService.Data);

            _distro.TryGetService<INetworkMessageHandler>(out var handlerService);
            Assert.AreSame(_handler, handlerService.Data);
        }

        [TestMethod]
        public void CanMoveFIles() {
            StageTestFile();
            StageTestFile();
            StageTestFile();
            StageTestFile();
            StageTestFile();


            var successRatio =_distro.Pump(out var results);
            Console.WriteLine("DIR={0}", _output.Parent);
            Console.WriteLine("OUTDIR={0}", _output);

            Console.WriteLine("SR={0}", successRatio);
            ReportResults(results);

            Assert.AreEqual(1.0,successRatio);
        }

        private void StageTestFile() {
            var inputFile = CreateTestInputFIle(out var originalDescription);

            var stagingSuccess = _distro.Stage(new SimpleThreadLocalStream(
                    (d) => File.OpenRead(inputFile),
                    originalDescription
                ),
                out var stageRsults);

            ReportResults(stageRsults);
            Assert.IsTrue(stagingSuccess);
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
