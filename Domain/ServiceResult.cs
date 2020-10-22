using System.Collections.Generic;

namespace BimspotTest
{
	public abstract class ServiceResult<TResult, TError, TErrorType> where TError : ErrorResult<TErrorType>
	{
		public TResult Result { get; set; }
		public bool Success { get; set; }
		public IEnumerable<TError> Errors { get; set; }
		protected ServiceResult(TResult result)
			: this(success: true, result: result, errors: null)
		{

		}
		protected ServiceResult(TError error)
			: this(success: false, result: default(TResult), errors: new List<TError>() { error })
		{

		}
		protected ServiceResult(IEnumerable<TError> errors)
			: this(success: false, result: default(TResult), errors: errors)
		{

		}
		protected ServiceResult(bool success, TResult result, IEnumerable<TError> errors)
		{
			this.Success = success;
			this.Result = result;
			this.Errors = errors;
		}
	}
}
