//-----------------------------------------------------------------------// <copyright file="Int32Serializer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
    /// <summary>
    /// Serializer for the <see cref="int"/> type.
    /// </summary>
    /// <seealso cref="Serializer{System.Int32}" />
    public sealed class Int32Serializer : Serializer<int>
    {
        /// <summary>
        /// Reads a value of type <see cref="int" />.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>
        /// The value which has been read.
        /// </returns>
        public override int ReadValue(IDataReader reader)
        {
            string name;
            var entry = reader.PeekEntry(out name);

            if (entry == EntryType.Integer)
            {
                int value;
                if (reader.ReadInt32(out value) == false)
                {
                    reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + name + "' of type " + entry.ToString());
                }
                return value;
            }
            else
            {
                reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer.ToString() + ", but got entry '" + name + "' of type " + entry.ToString());
                reader.SkipEntry();
                return default(int);
            }
        }

        /// <summary>
        /// Writes a value of type <see cref="int" />.
        /// </summary>
        /// <param name="name">The name of the value to write.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="writer">The writer to use.</param>
        public override void WriteValue(string name, int value, IDataWriter writer)
        {
            FireOnSerializedType();
            writer.WriteInt32(name, value);
        }
    }
}