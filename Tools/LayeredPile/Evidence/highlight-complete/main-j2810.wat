  (func $j2810 (type 1) (param i32 i32 i32)
    (local f32)
    i32.const 4505041
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3534772
      call $j870 ;; 
      i32.const 3534844
      call $j870 ;; 
      i32.const 4505041
      i32.const 1
      i32.store8
    end
    i32.const 0
    call $j1337 ;; 
    i32.eqz
    if  ;; label = @1
      local.get 0
      local.get 1
      i32.store8 offset=57
      return
    end
    local.get 0
    i32.load offset=476
    i32.eqz
    if  ;; label = @1
      local.get 0
      i32.const 0
      call $j1519 ;; 
      f32.store offset=472
    end
    block  ;; label = @1
      block  ;; label = @2
        local.get 1
        i32.eqz
        br_if 0 (;@2;)
        local.get 0
        i32.load8_u offset=56
        br_if 0 (;@2;)
        local.get 0
        i32.load8_u offset=57
        if  ;; label = @3
          local.get 0
          i32.load offset=476
          i32.eqz
          br_if 2 (;@1;)
        end
        local.get 0
        i32.load offset=312
        local.set 1
        local.get 1
        if  ;; label = @3
          local.get 0
          i32.const 0
          call $j925 ;; 
          local.set 2
          local.get 1
          i32.load offset=12
          local.get 1
          i32.load offset=32
          local.get 2
          local.get 1
          i32.load offset=20
          call $redir_iiii
          i32.eqz
          br_if 2 (;@1;)
        end
        i32.const 0
        local.set 1
        local.get 0
        i32.const 3534844
        i32.load
        i32.const 0
        i32.const 1
        i32.const 0
        call $j13977 ;; 
        i32.const 0
        call $j1519 ;; 
        local.set 3
        local.get 0
        local.get 3
        f32.store offset=428
        local.get 0
        local.get 3
        f32.store offset=432
        block  ;; label = @3
          local.get 0
          f32.load offset=60
          f32.const 0x0p+0 (;=0;)
          f32.gt
          i32.eqz
          br_if 0 (;@3;)
          i32.const 1
          local.set 1
          local.get 0
          i32.load offset=476
          i32.const -1
          i32.ne
          br_if 0 (;@3;)
          local.get 0
          f32.load offset=64
          i32.const 0
          call $j1519 ;; 
          local.get 0
          f32.load offset=472
          f32.sub
          f32.sub
          local.set 3
          local.get 0
          i32.const 0
          call $j1519 ;; 
          local.get 3
          f32.sub
          f32.store offset=472
        end
        local.get 0
        i32.const 1
        i32.store8 offset=748
        local.get 0
        i32.const 1
        i32.store8 offset=57
        local.get 0
        local.get 1
        i32.store offset=476
        return
      end
      local.get 0
      i32.load8_u offset=57
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      f32.load offset=64
      f32.const 0x0p+0 (;=0;)
      f32.gt
      if  ;; label = @2
        local.get 0
        i32.load offset=476
        i32.const 1
        i32.eq
        if  ;; label = @3
          i32.const 0
          call $j1519 ;; 
          local.get 0
          f32.load offset=472
          f32.sub
          local.set 3
          local.get 0
          i32.const 0
          call $j1519 ;; 
          local.get 3
          f32.add
          local.get 0
          f32.load offset=60
          f32.sub
          f32.store offset=472
        end
        local.get 0
        i32.const -1
        i32.store offset=476
        return
      end
      local.get 0
      i32.const 0
      i32.store8 offset=57
      local.get 0
      i32.const 0
      i32.store offset=476
      local.get 0
      i32.load offset=316
      local.set 1
      local.get 1
      if  ;; label = @2
        local.get 0
        i32.const 0
        call $j925 ;; 
        local.set 2
        local.get 1
        i32.load offset=12
        local.get 1
        i32.load offset=32
        local.get 2
        local.get 1
        i32.load offset=20
        call $redir_iiii
        drop
      end
      local.get 0
      i32.const 3534772
      i32.load
      i32.const 0
      i32.const 1
      i32.const 0
      call $j13977 ;; 
      local.get 0
      i32.const 1
      i32.store8 offset=748
    end)
