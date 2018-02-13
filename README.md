# [UNITY] procedural planet octree renderer
dual contour octree realtime renderer for point cloud data or procedural planet generation  
there's still work to do in order to fix the cracks between contiguous chunks with different level-of-detail (these gaps in the screenshots: once I used a quadtree approach I managed to obtain full, seamless stitching, then once I moved to an octree this thing became a bit brainy)
the algorithm used is basically dual contouring, but at the moment instead of calculating the quadratic error function 
I just average all the intersection points for each cell and then perform the geometry tesselation along these points, as at this stage there are no noticeable artifacts because of this oversimplification.  

![random point clound](https://raw.githubusercontent.com/paolorussian/proceduralplanet/master/ss/ss_pointcloud.png)
![rounded](https://raw.githubusercontent.com/paolorussian/proceduralplanet/master/ss/ss_rounded.png)
