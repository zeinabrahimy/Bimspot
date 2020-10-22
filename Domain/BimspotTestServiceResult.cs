using System.Collections.Generic;

namespace BimspotTest
{
	public class BimspotTestServiceResult<TResult> : ServiceResult<TResult, BimspotTestErrorResult, Enumeration.ErrorType>
	{
		public BimspotTestServiceResult(TResult result) : base(success: true, result: result, errors: null)
		{
		}

		public BimspotTestServiceResult(IEnumerable<BimspotTestErrorResult> errors) : base(success: false, result: default(TResult), errors: errors)
		{
		}

		public BimspotTestServiceResult(BimspotTestErrorResult error) : base(success: false, result: default(TResult), errors: new List<BimspotTestErrorResult>() { error })
		{
		}

		public BimspotTestServiceResult(bool success, TResult result, IEnumerable<BimspotTestErrorResult> errors) : base(success, result, errors)
		{
		}

	}
}
