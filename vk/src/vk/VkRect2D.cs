using System;
using System.Collections.Generic;
using System.Text;

namespace Vulkan
{
    /// <summary>
    /// Structure specifying a two-dimensional subregion.
    /// </summary>
    public unsafe partial struct VkRect2D : IEquatable<VkRect2D>
    {
        /// <summary>
        /// An <see cref="VkRect2D"/> with all of its components set to zero.
        /// </summary>
        public static readonly VkRect2D Zero = new VkRect2D(VkOffset2D.Zero, VkExtent2D.Zero);

        /// <summary>
        /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
        /// </summary>
        /// <param name="offset">The offset component of the rectangle.</param>
        /// <param name="extent">The extent component of the rectangle.</param>
        public VkRect2D(VkOffset2D offset, VkExtent2D extent)
        {
            this.offset = offset;
            this.extent = extent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkRect2D"/> structure.
        /// </summary>
        /// <param name="extent">The extent component of the rectangle.</param>
        public VkRect2D(VkExtent2D extent)
        {
            this.offset = default(VkOffset2D);
            this.extent = extent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect2D"/> structure.
        /// </summary>
        /// <param name="x">The X component of the offset.</param>
        /// <param name="y">The Y component of the offset.</param>
        /// <param name="width">The width component of the extent.</param>
        /// <param name="height">The height component of the extent.</param>
        public VkRect2D(int x, int y, uint width, uint height)
        {
            this.offset = new VkOffset2D(x, y);
            this.extent = new VkExtent2D(width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect2D"/> structure.
        /// </summary>
        /// <param name="x">The X component of the offset.</param>
        /// <param name="y">The Y component of the offset.</param>
        /// <param name="width">The width component of the extent.</param>
        /// <param name="height">The height component of the extent.</param>
        public VkRect2D(int x, int y, int width, int height)
        {
            this.offset = new VkOffset2D(x, y);
            this.extent = new VkExtent2D(width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect2D"/> structure.
        /// </summary>
        /// <param name="width">The width component of the extent.</param>
        /// <param name="height">The height component of the extent.</param>
        public VkRect2D(uint width, uint height)
        {
            this.offset = default(VkOffset2D);
            this.extent = new VkExtent2D(width, height);
        }

        /// <summary>
        /// Determines whether the specified <see cref="VkRect2D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkRect2D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkOffset2D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref VkRect2D other)
        {
            return other.offset.Equals(ref offset) && other.extent.Equals(ref extent);
        }

        /// <summary>
        /// Determines whether the specified <see cref="VkRect2D"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="VkRect2D"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="VkRect2D"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(VkRect2D other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is VkRect2D && Equals((VkRect2D)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = extent.GetHashCode();
                hashCode = (hashCode * 397) ^ offset.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given rectangles are equal.
        /// </summary>
        /// <param name="left">The first rectangle to compare.</param>
        /// <param name="right">The second rectangle to compare.</param>
        /// <returns><c>true</c> if the rectangles are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(VkRect2D left, VkRect2D right) => left.Equals(right);

        /// <summary>
        /// Returns a boolean indicating whether the two given rectangles are not equal.
        /// </summary>
        /// <param name="left">The first rectangle to compare.</param>
        /// <param name="right">The second rectangle to compare.</param>
        /// <returns>
        /// <c>true</c> if the rectangles are not equal; <c>false</c> if they are equal.
        /// </returns>
        public static bool operator !=(VkRect2D left, VkRect2D right) => !left.Equals(right);
    }
}
