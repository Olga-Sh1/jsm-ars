using JSMBase;
using JSMBase.Medicine;
using JSMBase.Medicine.Models;
using JSMSolver.Research;
using JSMSolver.Research.v1;
using JSMSolver.Research.Builders;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JSMSolver;
using JSMSolver.Research.Builders.v1;
using JSMSolver.Research.v1.Partitions;

namespace ERTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var researcher = new Researcher<JSMMedicine, JSMBase.JSMBase>();
            String[] data_path = new string[]
            {
                @"C:\TFS\JSM\Data\Worlds+8\W1\92.txt",
                @"C:\TFS\JSM\Data\Worlds+8\W1\133.txt",
                @"C:\TFS\JSM\Data\Worlds+8\W1\GASTRO158.TXT"
            };
            String[] pathes = new String[]
            {
                @"C:\TFS\JSM\Data\Worlds+8\W1\Results\X1_1.hs",
                @"C:\TFS\JSM\Data\Worlds+8\W1\Results\X2_1.hs",
                @"C:\TFS\JSM\Data\Worlds+8\W1\Results\X3_1.hs"
            };

            HypBatchFull[] hyps = new HypBatchFull[data_path.Length];
            for (int i = 0; i < data_path.Length; i++)
                hyps[i] = new HypBatchFull(data_path[i]);
            
            var builder = new ResearchDataHypBuilder<JSMMedicine, JSMBase.JSMBase>();
            for(int i = 0; i < data_path.Length; i++)
                builder.Add(hyps[i], pathes[i]);
            var arrh = await builder.Build();

            var builder2 = new ResearchDataBuilder<JSMMedicine, JSMBase.JSMBase>().AddThreshold(0.7);
            for (int i = 0; i < data_path.Length; i++)
                builder2.Add(hyps[i].WrappedCollection);
            var datas = builder2.Build();

            //var chains = await researcher.Count(CancellationToken.None, null, JSMSolver.Addings.Contr, JSMSolver.Addings.Contr, datas);
            var chains2 = await researcher.Count(CancellationToken.None, null, arrh.ToArray(), null);
            foreach (var chain in chains2.Data)
            {

            }
            var exp = chains2.Explicability[0][Signs.Plus];

            String[] pathes2 = new String[]
            {
                @"C:\TFS\JSM\Data\158+8+9t\X1_1.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_2.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_3.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_4.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_5.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_6.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_7.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_8.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_9.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_10.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_11.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_12.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_13.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_14.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_15.hs",
                @"C:\TFS\JSM\Data\158+8+9t\X1_16.hs",
            };
            var analyser = new PartitionAnalyser<JSMMedicine, JSMBase.JSMBase, StrongInconsistencyWrapper<JSMMedicine, JSMBase.JSMBase>>();
            PartitionInputDataBuilder<JSMMedicine, JSMBase.JSMBase> builder3 = new PartitionInputDataBuilder<JSMMedicine, JSMBase.JSMBase>();
            Addings[] adds = new Addings[] { Addings.Simple, Addings.Contr, Addings.Difference, Addings.Contr & Addings.Difference };
            for(int i = 0; i < adds.Length; i++)
            {
                for(int j = 0; j < adds.Length;j++)
                {
                    builder3.Add(adds[i], adds[j], new HypBatchFull(data_path[0]), pathes2[i * adds.Length + j]);
                }
            }
            var input = await builder3.Build();
            var all = builder3.CollectAllPreds();
            var parts = analyser.BuildPartition(all,input);
        }
    }
}
