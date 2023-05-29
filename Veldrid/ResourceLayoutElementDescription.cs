using System;

namespace Veldrid
{
    /// <summary>
    /// Describes an individual resource element in a <see cref="ResourceLayout"/>.
    /// </summary>
    public struct ResourceLayoutElementDescription : IEquatable<ResourceLayoutElementDescription>
    {
        /// <summary>
        /// The name of the element.
        /// </summary>
        public string Name;
        /// <summary>
        /// The kind of resource.
        /// </summary>
        public ResourceKind Kind;
        /// <summary>
        /// The <see cref="ShaderStages"/> in which this element is used.
        /// </summary>
        public ShaderStages Stages;
        /// <summary>
        /// Miscellaneous resource options for this element.
        /// </summary>
        public ResourceLayoutElementOptions Options;
        /// <summary>
        /// The number of descriptors to bind to this resource. Used for arrays of textures.
        /// </summary>
        public uint DescriptorCount;

        /// <summary>
        /// Constructs a new ResourceLayoutElementDescription.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="kind">The kind of resource.</param>
        /// <param name="stages">The <see cref="ShaderStages"/> in which this element is used.</param>
        public ResourceLayoutElementDescription(string name, ResourceKind kind, ShaderStages stages)
        {
            Name = name;
            Kind = kind;
            Stages = stages;
            Options = ResourceLayoutElementOptions.None;
            DescriptorCount = 1;
        }

        /// <summary>
        /// Constructs a new ResourceLayoutElementDescription.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="kind">The kind of resource.</param>
        /// <param name="stages">The <see cref="ShaderStages"/> in which this element is used.</param>
        /// <param name="descCount">The number of descriptors to use.</param>
        /// <param name="options">Miscellaneous resource options for this element.</param>
        public ResourceLayoutElementDescription(
            string name, 
            ResourceKind kind, 
            ShaderStages stages,
            ResourceLayoutElementOptions options,
            uint descCount)
        {
            Name = name;
            Kind = kind;
            Stages = stages;
            Options = options;
            DescriptorCount = descCount;
        }

        /// <summary>
        /// Constructs a new ResourceLayoutElementDescription.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="kind">The kind of resource.</param>
        /// <param name="stages">The <see cref="ShaderStages"/> in which this element is used.</param>
        /// <param name="options">Miscellaneous resource options for this element.</param>
        public ResourceLayoutElementDescription(
            string name,
            ResourceKind kind,
            ShaderStages stages,
            ResourceLayoutElementOptions options)
        {
            Name = name;
            Kind = kind;
            Stages = stages;
            Options = options;
            DescriptorCount = 1;
        }

        /// <summary>
        /// Element-wise equality.
        /// </summary>
        /// <param name="other">The instance to compare to.</param>
        /// <returns>True if all elements are equal; false otherswise.</returns>
        public bool Equals(ResourceLayoutElementDescription other)
        {
            return Name.Equals(other.Name) && Kind == other.Kind && Stages == other.Stages && Options == other.Options;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return HashHelper.Combine(Name.GetHashCode(), (int)Kind, (int)Stages, (int)Options);
        }
    }

    /// <summary>
    /// Miscellaneous options for an element in a <see cref="ResourceLayout"/>.
    /// </summary>
    [Flags]
    public enum ResourceLayoutElementOptions
    {
        /// <summary>
        /// No special options.
        /// </summary>
        None,
        /// <summary>
        /// Can be applied to a buffer type resource (<see cref="ResourceKind.StructuredBufferReadOnly"/>,
        /// <see cref="ResourceKind.StructuredBufferReadWrite"/>, or <see cref="ResourceKind.UniformBuffer"/>), allowing it to be
        /// bound with a dynamic offset using <see cref="CommandList.SetGraphicsResourceSet(uint, ResourceSet, uint[])"/>.
        /// Offsets specified this way must be a multiple of <see cref="GraphicsDevice.UniformBufferMinOffsetAlignment"/> or
        /// <see cref="GraphicsDevice.StructuredBufferMinOffsetAlignment"/>.
        /// </summary>
        DynamicBinding = 1 << 0,
        /// <summary>
        /// Element has a variable descriptor count. This must be the last element in the layout.
        /// </summary>
        VariableCount = 1 << 1,
    }
}
