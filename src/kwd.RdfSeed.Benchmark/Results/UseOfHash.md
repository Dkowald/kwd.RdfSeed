
Change to include a pre-calculated hash for valueType and valueString
Use this to speed up when matching node to existing.

StringVsHashThenString:

Indicates a speed increase:
> (175.1-141.2) / 175.1 = 20%

Actual:

No hash:
 -Create 100X4 unique nodes of various types
| 156.4 us |  3.17 us |  9.30 us | 159.6 us | 13.6719 |     - |     - |  42.37 KB |

With hash:
 -Create 100X4 unique nodes of various types 
| 115.09 us | 0.842 us | 0.746 us | 16.2354 |     - |     - |  50.09 KB |

>Speed improvement: 25.89% (156.4-115.9)/156.4 

>Memory cost: 16.77% (50.9-42.37)/50.9 

The speed gain vs memory cost is close.

Cannot switch; since mem cost is based on added properties.

Considering the main memory saving is via re-use of existing nodes;
rather than small nodes.
This speed gain is considered worth it.
