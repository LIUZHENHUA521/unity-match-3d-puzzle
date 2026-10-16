  (func $j39433 (type 3) (param i32 i32 i32)
    (local i32 i32 i32 i32 i32)
    block  ;; label = @1
      local.get 0
      i32.load offset=212
      local.set 2
      local.get 2
      i32.eqz
      br_if 0 (;@1;)
      loop  ;; label = @2
        local.get 6
        local.get 2
        i32.load offset=12
        i32.ge_s
        br_if 1 (;@1;)
        local.get 2
        local.get 6
        i32.const 24
        i32.mul
        i32.add
        i32.const 16
        i32.add
        local.set 2
        local.get 1
        local.set 5
        local.get 5
        i32.load offset=8
        local.set 4
        local.get 5
        i32.load offset=12
        local.set 3
        local.get 4
        local.set 7
        local.get 2
        local.set 4
        local.get 4
        local.get 7
        i32.store offset=16
        local.get 4
        local.get 3
        i32.store offset=20
        local.get 5
        i32.load
        local.set 3
        local.get 5
        i32.load offset=4
        local.set 4
        local.get 3
        local.set 5
        local.get 2
        local.set 3
        local.get 3
        local.get 5
        i32.store offset=8
        local.get 3
        local.get 4
        i32.store offset=12
        local.get 6
        i32.const 1
        i32.add
        local.set 6
        local.get 0
        i32.load offset=212
        local.set 2
        br 0 (;@2;)
      end
      unreachable
    end
    local.get 1
    local.set 5
    local.get 5
    i32.load
    local.set 4
    local.get 5
    i32.load offset=4
    local.set 3
    local.get 4
    local.set 1
    local.get 0
    local.set 4
    local.get 4
    local.get 1
    i32.store offset=172
    local.get 4
    local.get 3
    i32.store offset=176
    local.get 5
    i32.load offset=8
    local.set 3
    local.get 5
    i32.load offset=12
    local.set 4
    local.get 3
    local.set 1
    local.get 0
    local.set 3
    local.get 3
    local.get 1
    i32.store offset=180
    local.get 3
    local.get 4
    i32.store offset=184
    local.get 3
    i32.const 0
    local.get 6
    call $j5908 ;; HighlightEffect.UpdateMaterialProperties)
