Water-flow-Union-find-algorithm
===============================

The water flow problem solved with the quick Union find algorithm

Manual:
Button "create":        creates a field of cells;
Numeric up-down 1:      columbs of cells (cellNumbX);
Numeric up-down 2:      rows of cells (cellNumbY);
Numeric up-down 3:      how many cells to be oppened by the program, used for random oppening and chance calculation;
Button "rand.open":     randomly open cells, after the first time it is pressed, it will reanme itself:
                        if the value in Numeric up-down 3 is zero (0) or the number of all the cells, all cells will
                        be oppened in a random order, otherwize, the chosen number of cells will be oppened in a random order;
Button "close cells":   closes all oppened cellsl;
Button "reset program": deletes the "field" of cells and then a new "field" with new sizes can be created;
Butoon "calc.chance":   calculates what is the chace for the "water to be flowing trough the field", if the chosen number of
                        cells are oppened; all "successful" ways, where the "water flows" will be shown
Button "pause calc.":   pauses the calculating;
Button "stop calc.":    stops the calculating;
Button "debug":         shows the last successfull sequence of oppened cells, is the sequence tester timer blocked and is the
                        generator thread blocked;

In order for the water to be flowing, at least one of cells of the lowes row must be connected to at least one cell of the top
row. Still, in the code it is checked, if one of the cells in the bottom is connected to a cell "start". Cell "start" is the
cell from where the water is comming from. When a cell from the top row is oppened, it gets connected to the "start" cell.

Gray cell:  closed cell;
White cell: oppened cell- it is not connected to "start" cell;
Blue cell:  filled cell- it is connected to "start" cell;
