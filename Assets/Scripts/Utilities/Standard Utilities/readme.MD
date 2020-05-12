# Utilities
## Data Types
### Bounds 2D

Act as a Vector2, except that field are min & max.

## Design patterns
 ### Singleton

> public class MyClass : Singleton<      MyClass    > {}

### State machine

OwnedState has a reference to the state's owner.
*Example: If we are using a state machine pattern in a CharacterController, the owner'd be the player.*

### Object Pooling

You need to create a MonoBehaviour and set the ObjectPooler component on it.

Then, you assign tag by prefab to create a pool.

Finally, here's how to **spawn** a GameObject:
> ObjectPooler.Instance.SpawnFromPool("objectTag", position, rotation);

And, here's how to **destroy** a GameObject:
> ObjectPooler.Instance.EnqueueGameObject("objectTag", objectToDestroy);

### Command

It's just an abstract class, that have a Execute abstract function.

## Extensions

### Array

Resize function, resize the array to have the same length as the enum count provided.

*Example: You have a enum that have 3 values*
> enum Emotion {Happy, Neutral, Angry}

*and you want an array to have the same length:*
> MyEnum.Resize(typeof(Emotion));

### Color

You can use SetAlpha to set the **alpha** of the Color:
> myColor.SetAlpha(0.1f);

You can also get the hexadecimal of a color by using:
 > myColor.ToHex();

### Graphic (eg. images)

You can set the alpha of a graphic by using:
> myGraphic.SetAlpha(0.5f);

You can fade a graphic by using:
> float fadeDuration = 1f;
> myGraphic.Fade(FadeType.FadeIn, fadeDuration);

### MonoBehaviour

You can **delay** a function by using:
> float taskDelay = 0.5f;
> this.ExecuteAfterTime(taskDelay, () => 
> {
>Debug.Log("some logs");
> });

You can also execute a function at the next frame by using:
> float taskDelay = 0.5f;
> this.ExecuteAfterFrame(taskDelay, () =>
> {
> Debug.Log("log on next frame");
> });

### NavMesh agent

Get if a nav agent has reached its destination:
> myNavAgent.HasReachedDestination()

### Queue Extension

Keep the size of the queue by removing eldest entry:
> int wantedSize = 5;
> myQueue.RemoveEldestEntry(wantedSize);

### String Extension

Create a Color from a hexadecimal string
> Color myColor = "ffffed".HexToColor();

### Texture2D

Draw a line by using:
> myTex.DrawLine(0, 0, 5, 5, Color.black);

### Transform

Getting nearest component by using
> transform.GetClosestComponent(arrayToCheck);

Destroy children:
> transform.DestroyChildren();

Execute a function for each children:
> transform.ActionForEachChildren((GameObject child) =>
> {
> Debug.Log(child.name);
> });

### Vector2

Set Vector2  to **absolute**:
> myVectorTwo.ToAbsolute();

Convert Vector2 to Vector2Int by RoundToInt:
> myVectorTwo.ToVector2Int();

### Vector3

Get Vector3 from a Vector2:
> myVectorTwo.ToXZ();

Clamp a Vector3:
> myVector.Clamp(0, 5);

Get closest point:
> myVector.GetClosestPoint(othersPoints);
