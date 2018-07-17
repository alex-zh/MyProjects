using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GUI.Common.RichTextBox.Classes
{    
    public static class RichTexBoxExtentions
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;
                    foreach (T obj in FindVisualChildren<T>(child))
                        yield return obj;
                }
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (DependencyObject depObj1 in Enumerable.OfType<DependencyObject>(LogicalTreeHelper.GetChildren(depObj)))
                {
                    if (depObj1 != null && depObj1 is T)
                        yield return (T)depObj1;
                    foreach (T obj in FindLogicalChildren<T>(depObj1))
                        yield return obj;
                }
            }
        }

        public static DependencyObject FindVisualTreeRoot(this DependencyObject initial)
        {
            DependencyObject dependencyObject1 = initial;
            DependencyObject dependencyObject2 = initial;
            for (; dependencyObject1 != null; dependencyObject1 = dependencyObject1 is Visual || dependencyObject1 is Visual3D ? VisualTreeHelper.GetParent(dependencyObject1) : LogicalTreeHelper.GetParent(dependencyObject1))
                dependencyObject2 = dependencyObject1;
            return dependencyObject2;
        }

        public static T FindVisualAncestor<T>(this DependencyObject dependencyObject) where T : class
        {
            DependencyObject reference = dependencyObject;
            do
            {
                reference = VisualTreeHelper.GetParent(reference);
            }
            while (reference != null && !(reference is T));
            return reference as T;
        }

        public static T FindLogicalAncestor<T>(this DependencyObject dependencyObject) where T : class
        {
            DependencyObject current = dependencyObject;
            do
            {
                DependencyObject reference = current;
                current = LogicalTreeHelper.GetParent(current) ?? VisualTreeHelper.GetParent(reference);
            }
            while (current != null && !(current is T));
            return current as T;
        }
    }
}
