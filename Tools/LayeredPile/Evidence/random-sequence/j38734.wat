  (func $j38734 (type 1) (param i32 i32 i32)
    (local i32 i32 i32 i32)
    global.get 0
    local.set 5
    local.get 5
    local.set 4
    local.get 2
    i32.load offset=16
    i32.load offset=96
    i32.const 2
    call $j874
    local.set 3
    local.get 3
    i32.load offset=20
    i32.const -1
    i32.le_s
    if  ;; label = @1
      local.get 3
      i32.load offset=128
      i32.const 8
      i32.sub
      local.set 3
    else
      i32.const 4
      local.set 3
    end
    local.get 4
    local.get 3
    i32.const 15
    i32.add
    i32.const -16
    i32.and
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    local.get 0
    i32.load offset=8
    local.set 6
    local.get 2
    i32.load offset=16
    i32.load offset=96
    i32.load offset=100
    i32.load
    local.get 2
    i32.load offset=16
    i32.load offset=96
    i32.load offset=100
    local.get 0
    call $j917
    local.set 0
    local.get 2
    i32.load offset=16
    i32.load offset=96
    i32.load offset=40
    i32.load
    local.get 2
    i32.load offset=16
    i32.load offset=96
    i32.load offset=40
    local.get 6
    local.get 0
    call $j1306
    local.set 0
    local.get 2
    i32.load offset=16
    i32.load offset=96
    i32.const 5
    call $j874
    i32.load offset=64
    local.set 2
    local.get 0
    local.get 2
    i32.load offset=92
    i32.add
    local.set 0
    local.get 4
    local.get 0
    local.get 0
    i32.const 8
    i32.sub
    local.get 2
    i32.load offset=88
    i32.load offset=20
    i32.const -1
    i32.gt_s
    select
    local.get 3
    call $j885
    drop
    local.get 1
    local.get 4
    local.get 3
    call $j885
    drop
    local.get 5
    global.set 0)