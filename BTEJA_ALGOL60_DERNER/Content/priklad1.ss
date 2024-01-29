BEGIN
    INT x;
    x := 7;
    INT c := 5/2;
    INT arr[1] := [5];
    FUNCTION fc (INT g) RETURN INT 
        BEGIN 
            g := g + g; 
            RETURN g;
        END;
        
    PROCEDURE pr (INT proc) 
        BEGIN
            proc := proc * 2;
            RETURN proc;
        END;
    write(arr[0]);
    write(x);
    STR y := "hello";
    STR o := fc(y);
    x := pr(x);
    write(x);
    write(o);
    INT i := 10;
    WHILE i > 5
        BEGIN
            i := i - 1;
            write(i);
        END;
    STRING super := "suer";
    IF super == "super" THEN 
        BEGIN
            write("yay");
        END;
    ELSE
        BEGIN
            write("nay");
        END;
        
    INT array[5][5] := [[0,1,2,3,4,5][11,22,33,44,55]];
        
    array[0][0] := 1;
    
    write(array[0][0]);
END;