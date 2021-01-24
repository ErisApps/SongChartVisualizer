namespace SongChartVisualizer.Models
{
	public class NpsInfo
	{
		public readonly float Nps;
		public readonly float FromTime;
		public readonly float ToTime;

		public NpsInfo(float nps, float fromTime, float toTime)
		{
			Nps = nps;
			FromTime = fromTime;
			ToTime = toTime;
		}
	}
}