# BasicallyLightsOut
I found that I'm basically implementing Lights out (see [Lights Out wikipedia](https://en.wikipedia.org/wiki/Lights_Out_(game))).
I didn't know about the existence of Lights out...

This version would include (done/basically done vs to do):
- [X] Normal mode (moves count based): different handmade puzzles with different grid size and even grid shape (still squares as unit element), separated into difficulty groups. Objective: solve in as few moves as possible.
- [X] Timed mode (rubik's cube like): random (non trivial) puzzles of selectable size (4x4, 5x5, 6x6, 7x7).  Objective: solve in the shortest possible time.
- [X] Create mode: create your puzzle grid shape/size and scramble, save it and play in normal or timed mode.
- [ ] Rubik's cube like stats for timed mode: keep track of best single, mean of 5 (12,100 or current) solutions.
- [X] Multiple color puzzle: cycle of more than 2 color

And other possible ideas (done/basically done vs to do):
- [ ] Different unit element shape: not only square
- [ ] Random cuts of a square: the puzzle "grid" is constructed by making random cuts of a basic square (or shape), the different resulting sections become the basic elements of the puzzle. Two elements that have one side in common will be connected.
- [ ] Leaderboard for timed mode.
- [ ] 1vs1 multiplayer for timed mode
