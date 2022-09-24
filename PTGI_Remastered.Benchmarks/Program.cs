using System.Security.Cryptography;
using System.Xml.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ILGPU.Runtime;
using PTGI_Remastered.Classes;
using PTGI_Remastered.Inputs;
using PTGI_Remastered.Structs;

namespace PTGI_Remastered.Benchmarks
{
	[MinColumn, MaxColumn, MemoryDiagnoser(false), MarkdownExporter, HtmlExporter, CsvExporter]
	public class PTGIBenchmark
	{
		private List<Polygon> Polygons { get; set; }
		private int Seed = 1234;
		private PTGI PathTracer { get; set; }
		[Params(1, 4, 8, 16)]
		public int BounceLimit { get; set; }
		[Params(1, 4, 8, 16, 32, 64, 128)]
		public int GridSize { get; set; }
		[Params(1, 10, 100, 500, 1000)]
		public int SampleCount { get; set; }
		

		public PTGIBenchmark()
		{
			var xml = File.ReadAllText(@"C:\Users\nishi\Documents\PTGI\interior.xml");
			var serializer = new XmlSerializer(typeof(List<Polygon>));
			using (var reader = new StringReader(xml))
			{
				Polygons = (List<Polygon>)serializer.Deserialize(reader);
			}
			
			PathTracer = new PTGI();
		}

		[Benchmark]
		public RenderResult Render()
		{
			var renderSpecification = new RenderSpecification()
			{
				BounceLimit = BounceLimit,
				DeviceId = 0,
				GridSize = GridSize,
				AcceleratorType = AcceleratorType.Cuda,
				ImageHeight = 640,
				ImageWidth = 800,
				Objects = Polygons.ToArray(),
				SampleCount = SampleCount,
				IgnoreEnclosedPixels = false,
				Seed = Seed
			};
			return PathTracer.PathTraceRender(renderSpecification);
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
		}
	}
}