  (func $j19262 (type 2) (param i32 i32) (result i32)
    i32.const 4507831
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428500
      call $j870 ;; 
      i32.const 3434312
      call $j870 ;; 
      i32.const 4507831
      i32.const 1
      i32.store8
    end
    i32.const 3434312
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load8_u offset=80
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3428500
      i32.load
      call $j871 ;; 
      i32.const 0
      call $j4845 ;; ABPackManager.get_NewPlayerGuideABType
      br_if 0 (;@1;)
      i32.const 1
      return
    end
    i32.const 0
    return)
