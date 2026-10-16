  (func $j83882 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32 f32)
    i32.const 4511504
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3471296
      call $j870
      i32.const 3471300
      call $j870
      i32.const 4511504
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.load offset=32
    i32.load
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.load offset=32
    local.get 0
    i32.load offset=8
    call $j917
    i32.const 1
    i32.sub
    local.set 2
    local.get 0
    i32.const 0
    i32.store offset=28
    local.get 0
    local.get 2
    i32.store offset=32
    i32.const 0
    local.set 2
    loop  ;; label = @1
      block  ;; label = @2
        local.get 1
        i32.load offset=16
        i32.load offset=96
        i32.load offset=32
        i32.load
        local.get 1
        i32.load offset=16
        i32.load offset=96
        i32.load offset=32
        local.get 0
        i32.load offset=8
        call $j917
        local.set 3
        local.get 0
        f32.load offset=28
        local.set 6
        local.get 2
        local.get 3
        i32.ge_s
        br_if 0 (;@2;)
        local.get 1
        i32.load offset=16
        i32.load offset=96
        i32.load offset=40
        i32.load
        local.get 1
        i32.load offset=16
        i32.load offset=96
        i32.load offset=40
        local.get 0
        i32.load offset=8
        local.get 2
        call $j1306
        local.set 3
        local.get 1
        i32.load offset=16
        i32.load offset=96
        i32.const 5
        call $j874
        i32.load offset=64
        local.set 4
        local.get 3
        local.get 4
        i32.load offset=32
        i32.add
        local.set 3
        local.get 6
        local.get 3
        local.get 3
        i32.const 8
        i32.sub
        local.get 4
        i32.load offset=28
        i32.load offset=20
        i32.const -1
        i32.gt_s
        select
        f32.load
        f32.add
        local.set 6
        local.get 0
        local.get 6
        f32.store offset=28
        local.get 0
        i32.load offset=12
        local.set 4
        i32.const 3471300
        i32.load
        local.set 5
        global.get 0
        i32.const 16
        i32.sub
        local.set 3
        local.get 3
        global.set 0
        local.get 3
        local.get 6
        f32.store offset=12
        local.get 4
        local.get 2
        local.get 3
        i32.const 12
        i32.add
        local.get 5
        call $j13192
        local.get 3
        i32.const 16
        i32.add
        global.set 0
        local.get 2
        i32.const 1
        i32.add
        local.set 2
        br 1 (;@1;)
      end
    end
    f32.const 0x0p+0 (;=0;)
    local.get 6
    i32.const 0
    call $j1345
    local.set 6
    local.get 0
    local.get 0
    i32.load offset=20
    i32.const 1
    i32.add
    i32.store offset=20
    i32.const 0
    local.set 2
    loop  ;; label = @1
      local.get 1
      i32.load offset=16
      i32.load offset=96
      i32.load offset=32
      i32.load
      local.get 1
      i32.load offset=16
      i32.load offset=96
      i32.load offset=32
      local.get 0
      i32.load offset=8
      call $j917
      local.get 2
      i32.gt_s
      if  ;; label = @2
        local.get 0
        i32.load offset=12
        local.get 2
        i32.const 3471296
        i32.load
        call $j4276
        local.get 6
        f32.gt
        if  ;; label = @3
          local.get 0
          local.get 2
          i32.store offset=32
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=92
          local.set 2
          local.get 2
          i32.load offset=8
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=92
          i32.load
          local.get 2
          local.get 0
          i32.const 0
          i32.const 0
          call $redir_viiiii
        else
          local.get 2
          i32.const 1
          i32.add
          local.set 2
          br 2 (;@1;)
        end
      end
    end
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.load offset=96
    i32.load
    local.set 2
    local.get 1
    i32.load offset=16
    i32.load offset=96
    i32.load offset=96
    local.set 1
    local.get 1
    i32.load offset=8
    local.get 2
    local.get 1
    local.get 0
    i32.const 0
    i32.const 0
    call $redir_viiiii
    local.get 0
    i32.load offset=32
    return)
