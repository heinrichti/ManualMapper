# ManualMapper ![Build status](https://api.travis-ci.org/Lightraven/ManualMapper.svg?branch=master "Build status")
ManualMapper is really just a object-to-object mapping registry. 
If you like to know exactly what is happening but do not like to search for the right mapping-method then this might be for you!

It completely strips the "Auto"-part of many similar mapping projects like _AutoMapper_.

## Usage
``` c#
// Create instance of mapper. Usually you only need this one instance in your application.
var mapper = new Mapper()

// Create maps by supplying a mapping method
mapper.CreateMap<SourceType, TargetType>(MapSourceToTarget);

// Map your object to the other type just by providing the target type
TargetType mappedObj = mapper.Map<TargetType>(sourceObj);

// Example mapping function:
public TargetType MapSourceToTarget(SourceType src)
{
	return new TargetType 
	{
		DateOfBirth = src.DateOfBirth,
		FirstName = src.FirstName,
		LastName = src.LastName,
		NumberOfOrders = src.NumberOfOrders
	}
}
```

## Installation
To install ManualMapper run the following command in the Package Manager Console:
```
PM> Install-Package ManualMapper
```