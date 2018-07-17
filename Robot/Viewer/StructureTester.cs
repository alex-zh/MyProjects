using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common.Classes.Builders;
using Common.Classes.General;
using Common.Finders;

namespace Viewer
{
    internal class StructureTester
    {
        public const int LookForwardNumber = 31;
        public static string TestFolder = "C:\\MyProjects\\ResultData";
        private const string AllStructuresFilePath = "C:\\MyProjects\\ResultData\\allStructures.txt";
        public const string TestStructureResultFolderPath = "C:\\MyProjects\\ResultData\\found\\";

        private readonly List<Candle> _allCandles;

        public StructureTester(List<Candle> allCandles)
        {
            _allCandles = allCandles;
        }

        public void Test()
        {
            //FindAllStructures();
        }

        public List<Structure> FindAndSaveStructures()
        {
            var foundStructures = new List<Structure>();
                     
            var result = new List<StructureInfo>();

            var startPoints = GetStartPoints();

            var builder = new StructureBuilder(_allCandles);

            foreach (var startPoint in startPoints)
            {
                var structure = builder.Build(startPoint);

                if(structure == null) continue;
                
                var structureInfo = StructureParser.Parse(_allCandles, structure, false);

                if (structureInfo.Items.Sum(x => Math.Abs(x.Change)) > 0.002)
                {
                    result.Add(structureInfo);
                    foundStructures.Add(structure);
                }
            }

            new JsonSerializer<List<StructureInfo>>(AllStructuresFilePath).Write(result);

            return foundStructures;
        }

        private List<int> GetStartPoints()
        {
            var trendFinder = new EmaTrendFinder(_allCandles, 11, 21, 0.002, 8);
            var trends = trendFinder.FindAll();

            return trends.Where(x => x.StartIndex > 150).Select(x => x.StartIndex).ToList();
        }

        public void BuildStatistics()
        {
            var structures = new JsonSerializer<List<StructureInfo>>(AllStructuresFilePath).Read();

            foreach (var patternStructure in structures)
            {
                var result = GetTestStructureResult(patternStructure);
                if (result.Count <= 1)
                {
                    if (result.Count == 0)
                        // MessageBox.Show("Found only zero!");

                        continue;
                }

                var fileName = Path.Combine(TestStructureResultFolderPath, patternStructure.Id + ".txt");

                new JsonSerializer<TestStructureResult>(fileName).Write(result);
            }

        }

        public static StructureInfo FindStructureById(string id)
        {
            if (String.IsNullOrEmpty(id)) return null;

            var structures = new JsonSerializer<List<StructureInfo>>(AllStructuresFilePath).Read();

            return structures.First(x => x.Id == id);
        }

        private TestStructureResult GetTestStructureResult(StructureInfo patternStructure)
        {

            var structureFinder = new EmaRecursiveStructureFinder(_allCandles, patternStructure, false, true);
            var foundStructures = structureFinder.FindAll();

            var result = new TestStructureResult();

            foreach (var foundStructure in foundStructures)
            {
                var resultItem = new TestStructureResultItem
                {
                    StartIndex = foundStructure.StartIndex,
                    EndIndex = foundStructure.EndIndex,
                };

                if (resultItem.EndIndex + LookForwardNumber >= _allCandles.Count) continue;

                for (int i = resultItem.EndIndex + 1; i < resultItem.EndIndex + LookForwardNumber; i++)
                {
                    var simpleReturn = (int)(100000 * (_allCandles[i].Close - _allCandles[resultItem.EndIndex].Close) /
                                       _allCandles[resultItem.EndIndex].Close);

                    resultItem.Returns.Add(simpleReturn);
                }

                result.Add(resultItem);

            }

            return result;

        }

        public void BuildStatisticsFor(StructureInfo structure)
        {
            var fileName = Path.Combine(TestFolder, structure.Id + ".txt");

            if (File.Exists(fileName)) return;

            var result = GetTestStructureResult(structure);

            if (result.Count == 0)
            {
                 MessageBox.Show("Zero found!");
                return;
            }

           

            new JsonSerializer<TestStructureResult>(fileName).Write(result);

        }
    }
}