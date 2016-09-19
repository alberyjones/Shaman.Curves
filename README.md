# Shaman.Curves

Interpolates monotonic curves passing by the specified points.

```csharp
using Shaman.Curves;

var interpolator = new MonotoneCubicInterpolator();
interpolator.Add(0, 1); // At 0, the value must be 1
interpolator.Add(10, 5); // At 10, the value must be 5
interpolator.Add(14, 2); // And so on. Everything is automatically smoothed out.

interpolator.Interpolate(8); // Calculate the value for input 8.

```