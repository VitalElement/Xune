using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EmguFFmpeg
{
#if NET40
    /// <summary>
    /// IReadOnlyList interface for net40.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyList<T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// get element by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; }

        /// <summary>
        /// get count.
        /// </summary>
        int Count { get; }
    }
#endif

//    public partial class MediaIOContext2
//    {
//        public virtual byte[] ToArray()
//        {
//            int count = _length - _origin;
//            if (count == 0)
//                return new byte[0];
//#if NET40 || NETSTANDARD2_0
//            byte[] array = new byte[count];
//            Buffer.BlockCopy(_buffer, _origin, array, 0, count);
//            return array;
//#else
//            byte[] copy = GC.AllocateUninitializedArray<byte>(count);
//            _buffer.AsSpan(_origin, count).CopyTo(copy);
//            return copy;
//#endif
//        }

//#if !NET40
//        [NonSerialized]
//        private Task<int> _lastReadTask; // The last successful task returned from ReadAsync

//        public override Task FlushAsync(CancellationToken cancellationToken)
//        {
//            if (cancellationToken.IsCancellationRequested)
//                return Task.FromCanceled(cancellationToken);

//            try
//            {
//                Flush();
//                return Task.CompletedTask;
//            }
//            catch (Exception ex)
//            {
//                return Task.FromException(ex);
//            }
//        }

//        public override int Read(Span<byte> buffer)
//        {
//            if (GetType() != typeof(MediaIOContext2))
//            {
//                // MemoryStream is not sealed, and a derived type may have overridden Read(byte[], int, int) prior
//                // to this Read(Span<byte>) overload being introduced.  In that case, this Read(Span<byte>) overload
//                // should use the behavior of Read(byte[],int,int) overload.
//                return base.Read(buffer);
//            }

//            EnsureNotClosed();

//            int n = Math.Min(_length - _position, buffer.Length);
//            if (n <= 0)
//                return 0;

//            new Span<byte>(_buffer, _position, n).CopyTo(buffer);

//            _position += n;
//            return n;
//        }

//        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
//        {
//            ValidateBufferArguments(buffer, offset, count);

//            // If cancellation was requested, bail early
//            if (cancellationToken.IsCancellationRequested)
//                return Task.FromCanceled<int>(cancellationToken);

//            try
//            {
//                int n = Read(buffer, offset, count);
//                if (_lastReadTask == null || _lastReadTask.Result != n)
//                    return _lastReadTask = Task.FromResult(n);
//                else
//                    return _lastReadTask;
//            }
//            catch (OperationCanceledException oce)
//            {
//                return Task.FromCanceled<int>(oce.CancellationToken);
//            }
//            catch (Exception exception)
//            {
//                return Task.FromException<int>(exception);
//            }
//        }

//        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
//        {
//            if (cancellationToken.IsCancellationRequested)
//            {
//                return ValueTask.FromCanceled<int>(cancellationToken);
//            }

//            try
//            {
//                // ReadAsync(Memory<byte>,...) needs to delegate to an existing virtual to do the work, in case an existing derived type
//                // has changed or augmented the logic associated with reads.  If the Memory wraps an array, we could delegate to
//                // ReadAsync(byte[], ...), but that would defeat part of the purpose, as ReadAsync(byte[], ...) often needs to allocate
//                // a Task<int> for the return value, so we want to delegate to one of the synchronous methods.  We could always
//                // delegate to the Read(Span<byte>) method, and that's the most efficient solution when dealing with a concrete
//                // MemoryStream, but if we're dealing with a type derived from MemoryStream, Read(Span<byte>) will end up delegating
//                // to Read(byte[], ...), which requires it to get a byte[] from ArrayPool and copy the data.  So, we special-case the
//                // very common case of the Memory<byte> wrapping an array: if it does, we delegate to Read(byte[], ...) with it,
//                // as that will be efficient in both cases, and we fall back to Read(Span<byte>) if the Memory<byte> wrapped something
//                // else; if this is a concrete MemoryStream, that'll be efficient, and only in the case where the Memory<byte> wrapped
//                // something other than an array and this is a MemoryStream-derived type that doesn't override Read(Span<byte>) will
//                // it then fall back to doing the ArrayPool/copy behavior.
//                return new ValueTask<int>(
//                    MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> destinationArray) ?
//                        Read(destinationArray.Array!, destinationArray.Offset, destinationArray.Count) :
//                        Read(buffer.Span));
//            }
//            catch (OperationCanceledException oce)
//            {
//                return new ValueTask<int>(Task.FromCanceled<int>(oce.CancellationToken));
//            }
//            catch (Exception exception)
//            {
//                return ValueTask.FromException<int>(exception);
//            }
//        }

//        public override void Write(ReadOnlySpan<byte> buffer)
//        {
//            if (GetType() != typeof(MediaIOContext2))
//            {
//                // MemoryStream is not sealed, and a derived type may have overridden Write(byte[], int, int) prior
//                // to this Write(Span<byte>) overload being introduced.  In that case, this Write(Span<byte>) overload
//                // should use the behavior of Write(byte[],int,int) overload.
//                base.Write(buffer);
//                return;
//            }

//            EnsureNotClosed();
//            EnsureWriteable();

//            // Check for overflow
//            int i = _position + buffer.Length;
//            if (i < 0)
//                throw new IOException(SR.IO_StreamTooLong);

//            if (i > _length)
//            {
//                bool mustZero = _position > _length;
//                if (i > _capacity)
//                {
//                    bool allocatedNewArray = EnsureCapacity(i);
//                    if (allocatedNewArray)
//                    {
//                        mustZero = false;
//                    }
//                }
//                if (mustZero)
//                {
//                    Array.Clear(_buffer, _length, i - _length);
//                }
//                _length = i;
//            }

//            buffer.CopyTo(new Span<byte>(_buffer, _position, buffer.Length));
//            _position = i;
//        }

//        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
//        {
//            ValidateBufferArguments(buffer, offset, count);

//            // If cancellation is already requested, bail early
//            if (cancellationToken.IsCancellationRequested)
//                return Task.FromCanceled(cancellationToken);

//            try
//            {
//                Write(buffer, offset, count);
//                return Task.CompletedTask;
//            }
//            catch (OperationCanceledException oce)
//            {
//                return Task.FromCanceled(oce.CancellationToken);
//            }
//            catch (Exception exception)
//            {
//                return Task.FromException(exception);
//            }
//        }

//        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
//        {
//            if (cancellationToken.IsCancellationRequested)
//            {
//                return ValueTask.FromCanceled(cancellationToken);
//            }

//            try
//            {
//                // See corresponding comment in ReadAsync for why we don't just always use Write(ReadOnlySpan<byte>).
//                // Unlike ReadAsync, we could delegate to WriteAsync(byte[], ...) here, but we don't for consistency.
//                if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> sourceArray))
//                {
//                    Write(sourceArray.Array!, sourceArray.Offset, sourceArray.Count);
//                }
//                else
//                {
//                    Write(buffer.Span);
//                }
//                return default;
//            }
//            catch (OperationCanceledException oce)
//            {
//                return new ValueTask(Task.FromCanceled(oce.CancellationToken));
//            }
//            catch (Exception exception)
//            {
//                return ValueTask.FromException(exception);
//            }
//        }

//        public override void CopyTo(Stream destination, int bufferSize)
//        {
//            // If we have been inherited into a subclass, the following implementation could be incorrect
//            // since it does not call through to Read() which a subclass might have overridden.
//            // To be safe we will only use this implementation in cases where we know it is safe to do so,
//            // and delegate to our base class (which will call into Read) when we are not sure.
//            if (GetType() != typeof(MediaIOContext2))
//            {
//                base.CopyTo(destination, bufferSize);
//                return;
//            }

//            // Validate the arguments the same way Stream does for back-compat.
//            ValidateCopyToArguments(destination, bufferSize);
//            EnsureNotClosed();

//            int originalPosition = _position;

//            // Seek to the end of the MemoryStream.
//            int remaining = InternalEmulateRead(_length - originalPosition);

//            // If we were already at or past the end, there's no copying to do so just quit.
//            if (remaining > 0)
//            {
//                // Call Write() on the other Stream, using our internal buffer and avoiding any
//                // intermediary allocations.
//                destination.Write(_buffer, originalPosition, remaining);
//            }
//        }

//        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
//        {
//            // This implementation offers better performance compared to the base class version.

//            ValidateCopyToArguments(destination, bufferSize);
//            EnsureNotClosed();

//            // If we have been inherited into a subclass, the following implementation could be incorrect
//            // since it does not call through to ReadAsync() which a subclass might have overridden.
//            // To be safe we will only use this implementation in cases where we know it is safe to do so,
//            // and delegate to our base class (which will call into ReadAsync) when we are not sure.
//            if (GetType() != typeof(MediaIOContext2))
//                return base.CopyToAsync(destination, bufferSize, cancellationToken);

//            // If canceled - return fast:
//            if (cancellationToken.IsCancellationRequested)
//                return Task.FromCanceled(cancellationToken);

//            // Avoid copying data from this buffer into a temp buffer:
//            // (require that InternalEmulateRead does not throw,
//            // otherwise it needs to be wrapped into try-catch-Task.FromException like memStrDest.Write below)

//            int pos = _position;
//            int n = InternalEmulateRead(_length - _position);

//            // If we were already at or past the end, there's no copying to do so just quit.
//            if (n == 0)
//                return Task.CompletedTask;

//            // If destination is not a memory stream, write there asynchronously:
//            if (!(destination is MediaIOContext2 memStrDest))
//                return destination.WriteAsync(_buffer, pos, n, cancellationToken);

//            try
//            {
//                // If destination is a MemoryStream, CopyTo synchronously:
//                memStrDest.Write(_buffer, pos, n);
//                return Task.CompletedTask;
//            }
//            catch (Exception ex)
//            {
//                return Task.FromException(ex);
//            }
//        }

//        private void ValidateCopyToArguments(Stream destination, int bufferSize)
//        {
//            if (destination == null)
//            {
//                throw new ArgumentNullException("destination");
//            }
//            if (bufferSize <= 0)
//            {
//                throw new ArgumentOutOfRangeException("bufferSize", SR.ArgumentOutOfRange_NeedPosNum);
//            }
//            if (!this.CanRead && !this.CanWrite)
//            {
//                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
//            }
//            if (!destination.CanRead && !destination.CanWrite)
//            {
//                throw new ObjectDisposedException("destination", SR.ObjectDisposed_StreamClosed);
//            }
//            if (!this.CanRead)
//            {
//                throw new NotSupportedException(SR.NotSupported_UnreadableStream);
//            }
//            if (!destination.CanWrite)
//            {
//                throw new NotSupportedException(SR.NotSupported_UnwritableStream);
//            }
//        }
//#endif
//    }
}
