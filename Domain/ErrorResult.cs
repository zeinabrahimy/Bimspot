namespace BimspotTest
{
	public abstract class ErrorResult<TErrorType>
	{
		public TErrorType Type { get; set; }
		public string Message { get; set; }
	}
}
