  (func $j19885 (type 38) (param i32 i32 f32 i32 i32)
    (local i32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 5
    local.get 5
    global.set 0
    i32.const 4509339
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3470264
      call $j870
      i32.const 3470268
      call $j870
      i32.const 4509339
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.set 4
    loop  ;; label = @1
      i32.const 3470264
      i32.load
      drop
      local.get 4
      local.get 1
      i32.load offset=12
      i32.lt_s
      if  ;; label = @2
        block  ;; label = @3
          local.get 1
          local.get 4
          i32.const 3470268
          i32.load
          call $j895
          i32.load8_u offset=18
          br_if 0 (;@3;)
          local.get 5
          local.get 1
          local.get 4
          i32.const 3470268
          i32.load
          call $j895
          i32.const 0
          call $j944
          i32.const 0
          call $j1028
          local.get 5
          f32.load offset=4
          local.get 6
          f32.gt
          i32.eqz
          br_if 0 (;@3;)
          local.get 5
          local.get 1
          local.get 4
          i32.const 3470268
          i32.load
          call $j895
          i32.const 0
          call $j944
          i32.const 0
          call $j1028
          local.get 5
          f32.load offset=4
          local.set 6
        end
        local.get 4
        i32.const 1
        i32.add
        local.set 4
        br 1 (;@1;)
      end
    end
    local.get 2
    local.get 6
    f32.gt
    if  ;; label = @1
      local.get 0
      i32.const 300
      i32.const 364
      local.get 3
      i32.const 1
      i32.eq
      select
      i32.add
      local.get 6
      f32.store
      local.get 0
      local.get 6
      i32.const 0
      local.get 3
      local.get 4
      call $j9878
    end
    local.get 5
    i32.const 16
    i32.add
    global.set 0)
