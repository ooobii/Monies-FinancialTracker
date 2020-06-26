using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Swashbuckle.Swagger.Annotations
{
    
    /// <summary>
    /// Annotates a controller with a Swagger sorting order that is used when generating the Swagger documentation to
    /// order the controllers in a specific desired order.
    /// </summary>
    public class SwaggerControllerOrderAttribute : Attribute
    {
        /// <summary>
        /// Gets the sorting order of the controller.
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerControllerOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">Sets the sorting order of the controller.</param>
        public SwaggerControllerOrderAttribute(int order) {
            Order = order;
        }
    }


    /// <summary>
    /// Represents a controller name comparison operation that uses specific rules to determine the sort order of a
    /// controller when generating Swagger documentation.
    /// </summary>
    /// <typeparam name="T">The type controllers should implement (e.g. "ApiController")</typeparam>
    public class SwaggerControllerOrderComparer<T> : IComparer<string>
    {
        private readonly Dictionary<string, int> _orders;   // Our lookup table which contains controllername -> sortorder pairs

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerControllerOrderComparer&lt;TargetException&gt;"/> class.
        /// </summary>
        /// <param name="assembly">The assembly to scan for for classes implementing <typeparamref name="T"/>.</param>
        public SwaggerControllerOrderComparer(Assembly assembly)
            : this(GetFromAssembly<T>(assembly)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerControllerOrderComparer&lt;TargetException&gt;"/> class.
        /// </summary>
        /// <param name="controllers">
        /// The controllers to scan for a <see cref="SwaggerControllerOrderAttribute"/> to determine the sortorder.
        /// </param>
        public SwaggerControllerOrderComparer(IEnumerable<Type> controllers) {
            // Initialize our dictionary; scan the given controllers for our custom attribute, read the Order property
            // from the attribute and store it as controllername -> sorderorder pair in the (case-insensitive)
            // dicationary.
            _orders = new Dictionary<string, int>(
                controllers.Where(c => c.GetCustomAttributes<SwaggerControllerOrderAttribute>().Any())
                .Select(c => new { Name = ResolveControllerName(c.Name), c.GetCustomAttribute<SwaggerControllerOrderAttribute>().Order })
                .ToDictionary(v => v.Name, v => v.Order), StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns all <typeparamref name="TController"/>'s from the given assembly.
        /// </summary>
        /// <typeparam name="TController">The type classes must implement to be regarded a controller.</typeparam>
        /// <param name="assembly">The assembly to scan for given <typeparamref name="TController"/>s.</param>
        /// <returns>Returns all types implementing <typeparamref name="TController"/>.</returns>
        public static IEnumerable<Type> GetFromAssembly<TController>(Assembly assembly) {
            return assembly.GetTypes().Where(c => typeof(TController).IsAssignableFrom(c));
        }

        /// <summary>
        /// Compares to specified controller names and returns an integer that indicates their relative position in the
        /// sort order.
        /// </summary>
        /// <param name="controllerX">The first controller name to compare.</param>
        /// <param name="controllerY">The second controller name to compare.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relationship between the two controller positions. If
        /// controllerX precedes controllerY a value less than zero is returned. When controllerY precedes controllerX
        /// a value greater than zero is returned. When controllerX and controllerY occur in the same sort order the
        /// value zero is returned.
        /// </returns>
        public int Compare(string controllerX, string controllerY) {
            // Try, for both controllers, to get the sortorder value from our lookup; if none is found, assume int.MaxValue
            if( !_orders.TryGetValue(controllerX, out int xOrder) )
                xOrder = int.MaxValue;
            if( !_orders.TryGetValue(controllerY, out int yOrder) )
                yOrder = int.MaxValue;

            // If sortorder values differ, return the result
            if( xOrder != yOrder )
                return xOrder.CompareTo(yOrder);
            // If sort order values are equal, we fall back to ordering by name
            return string.Compare(controllerX, controllerY, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines the 'friendly' name of the controller by stripping the (by convention) "Controller" suffix
        /// from the name. If there's a built-in way to do this in .Net then I'd love to hear about it!
        /// </summary>
        /// <param name="name">The name of the controller.</param>
        /// <returns>The friendly name of the controller.</returns>
        private static string ResolveControllerName(string name) {
            const string suffix = "Controller"; // We want to strip "Controller" from "FooController"

            // Ensure name ends with suffix (case-insensitive)
            if( name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) )
                // Return name with suffix stripped
                return name.Substring(0, name.Length - suffix.Length);
            // Suffix not found, return name as-is
            return name;
        }
    }

}