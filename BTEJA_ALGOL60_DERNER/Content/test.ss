begin
    int x;
    x := 7;
    int c := 5/2;
    int arr[1] := [5];
    FUNCTION fc (int g) RETURN int 
        BEGIN 
            g := g + g; 
            RETURN g;
        END;
    write(arr[0]);
    write(x);
    str y := "hello";
    str o := fc(y);
    write(o);
end;