  (func $j38485 (type 0) (param i32 i32)
    (local i32 i32 f32 f32 f32 f64)
    global.get 0
    i32.const 16
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4509393
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3470264
      call $j870
      i32.const 3470268
      call $j870
      i32.const 4509393
      i32.const 1
      i32.store8
    end
    local.get 0
    f32.load offset=304
    local.set 4
    local.get 0
    f32.load offset=748
    local.set 5
    block  ;; label = @1
      local.get 4
      local.get 5
      f32.lt
      i32.eqz
      if  ;; label = @2
        local.get 0
        i32.load offset=744
        local.set 1
        br 1 (;@1;)
      end
      local.get 0
      local.set 3
      local.get 5
      local.get 4
      f32.sub
      local.get 0
      f32.load offset=736
      f32.div
      f64.promote_f32
      local.set 7
      block  ;; label = @2
        local.get 7
        f64.abs
        f64.const 0x1p+31 (;=2.14748e+09;)
        f64.lt
        if  ;; label = @3
          local.get 7
          call $asm2wasm_f64_to_int
          local.set 1
          br 1 (;@2;)
        end
        i32.const -2147483648
        local.set 1
      end
      local.get 3
      local.get 1
      i32.store offset=744
      local.get 0
      i32.load offset=740
      local.set 3
      i32.const 3
      local.get 1
      i32.sub
      local.set 1
      i32.const 0
      local.get 3
      local.get 1
      local.get 1
      local.get 3
      i32.gt_s
      select
      local.get 1
      i32.const 0
      i32.lt_s
      select
      local.set 1
      local.get 0
      local.get 1
      i32.store offset=744
    end
    local.get 0
    i32.load offset=740
    local.get 1
    i32.gt_s
    if  ;; label = @1
      local.get 0
      local.get 1
      i32.const 1
      i32.add
      i32.store offset=744
      local.get 0
      local.get 0
      f32.load offset=304
      local.get 0
      f32.load offset=736
      f32.sub
      i32.const 0
      i32.const 1
      local.get 0
      call $j9878
      local.get 0
      local.get 0
      f32.load offset=304
      f32.store offset=748
      local.get 0
      local.get 0
      call $j19882
    end
    local.get 0
    local.get 0
    call $j38480
    local.set 5
    i32.const 0
    local.set 1
    loop  ;; label = @1
      i32.const 3470264
      i32.load
      drop
      local.get 1
      local.get 0
      i32.load offset=192
      i32.load offset=12
      i32.lt_s
      if  ;; label = @2
        local.get 2
        local.get 0
        i32.load offset=192
        local.get 1
        i32.const 3470268
        i32.load
        call $j895
        i32.const 0
        call $j944
        i32.const 0
        call $j1028
        local.get 0
        f32.load offset=300
        local.set 4
        local.get 0
        f32.load offset=732
        local.set 6
        f32.const 0x1p+0 (;=1;)
        local.get 2
        f32.load offset=4
        local.get 4
        local.get 6
        f32.sub
        f32.sub
        local.get 5
        local.get 4
        local.get 6
        f32.sub
        f32.sub
        f32.div
        f32.sub
        local.set 4
        local.get 0
        i32.load offset=192
        local.get 1
        i32.const 3470268
        i32.load
        call $j895
        local.get 0
        f32.load offset=724
        f32.const 0x0p+0 (;=0;)
        local.get 4
        f32.const 0x1p+0 (;=1;)
        f32.min
        local.get 4
        f32.const 0x0p+0 (;=0;)
        f32.lt
        select
        local.get 0
        f32.load offset=728
        f32.mul
        f32.add
        i32.const 0
        call $j19261
        local.get 1
        i32.const 1
        i32.add
        local.set 1
        br 1 (;@1;)
      end
    end
    local.get 2
    i32.const 16
    i32.add
    global.set 0)
