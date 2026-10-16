  (func $j65349 (type 2) (param i32 i32) (result i32)
    i32.const 4507843
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3440336
      call $j870 ;; 
      i32.const 4507843
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          local.get 0
          i32.load offset=8
          br_table 0 (;@3;) 1 (;@2;) 2 (;@1;)
        end
        local.get 0
        i32.const -1
        i32.store offset=8
        i32.const 3440336
        i32.load
        call $j873 ;; 
        local.set 1
        local.get 1
        f32.const 0x1.99999ap-5 (;=0.05;)
        f32.store offset=8
        local.get 0
        local.get 1
        i32.store offset=12
        local.get 0
        i32.const 1
        i32.store offset=8
        i32.const 1
        return
      end
      local.get 0
      i32.load offset=16
      local.set 1
      local.get 0
      i32.const -1
      i32.store offset=8
      local.get 1
      i32.load8_u offset=16
      br_if 0 (;@1;)
      local.get 1
      i32.load offset=24
      i32.const 0
      i32.const 0
      call $j907 ;; GameObject.SetActive
      local.get 1
      i32.load offset=28
      i32.const 0
      i32.const 0
      call $j907 ;; GameObject.SetActive
    end
    i32.const 0
    return)
