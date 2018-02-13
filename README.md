# proceduralplanet
dual contour octree realtime renderer for point cloud data or procedural planet generation  
there's still work to do in order to fix the cracks between contiguous chunks with different level-of-detail
the algorithm used is basically dual contouring, but at the moment instead of calculating the quadratic error function 
I just average all the intersection points for each cell into one per cell, as at this stage there are no noticeable artifacts because of this oversimplification.  

![random point clound](https://raw.githubusercontent.com/paolorussian/proceduralplanet/master/ss/ss_pointcloud.png)
![rounded](https://raw.githubusercontent.com/paolorussian/proceduralplanet/master/ss/ss_rounded.png)
