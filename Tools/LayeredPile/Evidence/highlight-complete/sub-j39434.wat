  (func $j39434 (type 3) (param i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32 f32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4505057
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3452652
      i32.const 870
      call_indirect (type 0)
      i32.const 3452656
      i32.const 870
      call_indirect (type 0)
      i32.const 3432484
      i32.const 870
      call_indirect (type 0)
      i32.const 3473648
      i32.const 870
      call_indirect (type 0)
      i32.const 3473656
      i32.const 870
      call_indirect (type 0)
      i32.const 3473640
      i32.const 870
      call_indirect (type 0)
      i32.const 3473684
      i32.const 870
      call_indirect (type 0)
      i32.const 3473688
      i32.const 870
      call_indirect (type 0)
      i32.const 3425496
      i32.const 870
      call_indirect (type 0)
      i32.const 3495728
      i32.const 870
      call_indirect (type 0)
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 4505057
      i32.const 1
      i32.store8
    end
    local.get 2
    i32.const 0
    i32.store offset=44
    local.get 0
    i32.load offset=416
    local.set 3
    block  ;; label = @1
      local.get 3
      i32.eqz
      br_if 0 (;@1;)
      local.get 3
      i32.load offset=12
      local.get 1
      i32.le_s
      br_if 0 (;@1;)
      local.get 3
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      i32.load offset=68
      local.set 4
      local.get 4
      local.set 3
      local.get 3
      i32.load
      local.set 3
      local.get 3
      i32.load offset=204
      local.get 4
      local.get 3
      i32.load offset=208
      call $redir_iii
      local.set 7
      i32.const 3432484
      i32.load
      i32.const 871
      call_indirect (type 0)
      block  ;; label = @2
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=120
        local.get 7
        local.get 2
        i32.const 44
        i32.add
        i32.const 3452652
        i32.load
        i32.const 1125
        call_indirect (type 8)
        if  ;; label = @3
          local.get 2
          i32.load offset=44
          local.set 3
          i32.const 3435416
          i32.load
          i32.const 871
          call_indirect (type 0)
          local.get 3
          i32.const 0
          i32.const 0
          i32.const 924
          call_indirect (type 5)
          i32.eqz
          br_if 1 (;@2;)
        end
        local.get 4
        i32.const 0
        i32.const 13101
        call_indirect (type 2)
        i32.eqz
        br_if 1 (;@1;)
        i32.const 3432484
        i32.load
        i32.const 871
        call_indirect (type 0)
        block  ;; label = @3
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=100
          i32.eqz
          if  ;; label = @4
            i32.const 3425496
            i32.load
            i32.const 873
            call_indirect (type 1)
            local.set 3
            local.get 3
            i32.const 3473640
            i32.load
            i32.const 918
            call_indirect (type 4)
            i32.const 3432484
            i32.load
            i32.const 871
            call_indirect (type 0)
            i32.const 3432484
            i32.load
            i32.load offset=92
            local.get 3
            i32.store offset=100
            i32.const 3432484
            i32.load
            i32.load offset=92
            drop
            br 1 (;@3;)
          end
          i32.const 3432484
          i32.load
          i32.const 871
          call_indirect (type 0)
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=100
          i32.const 3473656
          i32.load
          i32.const 974
          call_indirect (type 4)
        end
        i32.const 3432484
        i32.load
        i32.const 871
        call_indirect (type 0)
        block  ;; label = @3
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=96
          i32.eqz
          if  ;; label = @4
            i32.const 3425496
            i32.load
            i32.const 873
            call_indirect (type 1)
            local.set 3
            local.get 3
            i32.const 3473640
            i32.load
            i32.const 918
            call_indirect (type 4)
            i32.const 3432484
            i32.load
            i32.const 871
            call_indirect (type 0)
            i32.const 3432484
            i32.load
            i32.load offset=92
            local.get 3
            i32.store offset=96
            i32.const 3432484
            i32.load
            i32.load offset=92
            drop
            br 1 (;@3;)
          end
          i32.const 3432484
          i32.load
          i32.const 871
          call_indirect (type 0)
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=96
          i32.const 3473656
          i32.load
          i32.const 974
          call_indirect (type 4)
        end
        i32.const 3432484
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 4
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=96
        i32.const 0
        i32.const 19575
        call_indirect (type 3)
        i32.const 3473684
        i32.load
        drop
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=96
        i32.load offset=12
        local.set 6
        local.get 6
        i32.eqz
        br_if 1 (;@1;)
        i32.const 4525920
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          i32.const 870
          call_indirect (type 0)
          i32.const 4525920
          i32.const 1
          i32.store8
        end
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 3
        local.get 3
        f32.load offset=8
        local.set 9
        local.get 3
        f32.load offset=4
        local.set 10
        local.get 3
        f32.load
        local.set 11
        i32.const 0
        local.set 3
        loop  ;; label = @3
          local.get 3
          local.get 6
          i32.lt_s
          if  ;; label = @4
            i32.const 3432484
            i32.load
            i32.const 871
            call_indirect (type 0)
            local.get 2
            i32.const 32
            i32.add
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=96
            local.get 3
            i32.const 3473688
            i32.load
            i32.const 986
            call_indirect (type 6)
            local.get 11
            local.get 2
            f32.load offset=32
            f32.add
            local.set 11
            local.get 10
            local.get 2
            f32.load offset=36
            f32.add
            local.set 10
            local.get 9
            local.get 2
            f32.load offset=40
            f32.add
            local.set 9
            local.get 3
            i32.const 1
            i32.add
            local.set 3
            br 1 (;@3;)
          end
        end
        local.get 9
        local.set 13
        local.get 6
        f32.convert_i32_s
        local.set 9
        local.get 13
        local.get 9
        f32.div
        local.set 14
        local.get 10
        local.get 9
        f32.div
        local.set 15
        local.get 11
        local.get 9
        f32.div
        local.set 13
        i32.const 0
        local.set 3
        loop  ;; label = @3
          local.get 3
          local.get 6
          i32.lt_s
          if  ;; label = @4
            i32.const 3432484
            i32.load
            i32.const 871
            call_indirect (type 0)
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=100
            local.set 8
            local.get 2
            i32.const 32
            i32.add
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=96
            local.get 3
            i32.const 3473688
            i32.load
            i32.const 986
            call_indirect (type 6)
            local.get 2
            f32.load offset=32
            local.get 13
            f32.sub
            local.set 9
            local.get 2
            f32.load offset=36
            local.get 15
            f32.sub
            local.set 10
            local.get 2
            f32.load offset=40
            local.get 14
            f32.sub
            local.set 11
            i32.const 4523251
            i32.load8_u
            i32.eqz
            if  ;; label = @5
              i32.const 3434688
              i32.const 870
              call_indirect (type 0)
              i32.const 4523251
              i32.const 1
              i32.store8
            end
            i32.const 3434688
            i32.load
            i32.const 871
            call_indirect (type 0)
            local.get 9
            local.get 9
            f32.mul
            local.get 10
            local.get 10
            f32.mul
            f32.add
            local.get 11
            local.get 11
            f32.mul
            f32.add
            f32.sqrt
            local.set 12
            block  ;; label = @5
              local.get 12
              f32.const 0x1.4f8b58p-17 (;=1e-05;)
              f32.gt
              if  ;; label = @6
                local.get 11
                local.get 12
                f32.div
                local.set 11
                local.get 10
                local.get 12
                f32.div
                local.set 10
                local.get 9
                local.get 12
                f32.div
                local.set 9
                br 1 (;@5;)
              end
              i32.const 4525920
              i32.load8_u
              i32.eqz
              if  ;; label = @6
                i32.const 3439908
                i32.const 870
                call_indirect (type 0)
                i32.const 4525920
                i32.const 1
                i32.store8
              end
              i32.const 3439908
              i32.load
              i32.load offset=92
              local.set 5
              local.get 5
              f32.load offset=8
              local.set 11
              local.get 5
              f32.load offset=4
              local.set 10
              local.get 5
              f32.load
              local.set 9
            end
            local.get 2
            local.get 11
            f32.store offset=24
            local.get 2
            local.get 2
            i32.load offset=24
            i32.store offset=8
            local.get 2
            local.get 10
            f32.store offset=20
            local.get 2
            local.get 9
            f32.store offset=16
            local.get 2
            i32.load offset=20
            local.set 5
            local.get 2
            local.get 2
            i32.load offset=16
            i32.store
            local.get 2
            local.get 5
            i32.store offset=4
            local.get 8
            local.get 2
            i32.const 3473648
            i32.load
            i32.const 903
            call_indirect (type 3)
            local.get 3
            i32.const 1
            i32.add
            local.set 3
            br 1 (;@3;)
          end
        end
        i32.const 3435416
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 4
        i32.const 3495728
        i32.load
        i32.const 1764
        call_indirect (type 2)
        local.set 3
        local.get 2
        local.get 3
        i32.store offset=44
        i32.const 3432484
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 3
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=100
        i32.const 0
        i32.const 8009
        call_indirect (type 3)
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=120
        local.get 7
        local.get 2
        i32.load offset=44
        i32.const 3452656
        i32.load
        i32.const 1769
        call_indirect (type 6)
      end
      local.get 0
      i32.load offset=416
      local.get 1
      i32.const 164
      i32.mul
      i32.add
      local.get 2
      i32.load offset=44
      i32.store offset=68
    end
    local.get 2
    i32.const 48
    i32.add
    global.set 0)
